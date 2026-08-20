"""
Standalone fallback client for Burglin' Gnomes -- only needed if you're not
running the BepInEx mod (BurglinGnomesArchipelago) in-game. If the mod is
installed, its F6 debug overlay + GameHooks.cs handle everything and you
don't need this at all.

Modeled on Archipelago's own minimal example client. API surface
(CommonContext, server_loop, etc.) matches recent Archipelago releases from
training knowledge -- not tested against a live server from this sandbox.
"""

import asyncio

from CommonClient import (
    CommonContext,
    ClientCommandProcessor,
    get_base_parser,
    gui_enabled,
    logger,
    server_loop,
)

from .Locations import ALL_TASK_NAMES, location_name_for_task, location_name_to_id


class BurglinGnomesCommandProcessor(ClientCommandProcessor):
    def _cmd_tasks(self):
        """List not-yet-completed High-Gnome tasks."""
        self.ctx.print_available_tasks()

    def _cmd_complete(self, *task_words):
        """Mark a task complete, e.g. /complete Cause Explosion"""
        name = " ".join(task_words).strip()
        self.ctx.complete_task(name)


class BurglinGnomesContext(CommonContext):
    game = "Burglin' Gnomes"
    command_processor = BurglinGnomesCommandProcessor
    items_handling = 0b111

    def __init__(self, server_address, password):
        super().__init__(server_address, password)
        self.checked_locations_local = set()

    async def server_auth(self, password_requested: bool = False):
        if password_requested and not self.password:
            await super().server_auth(password_requested)
        await self.get_username()
        await self.send_connect()

    def on_package(self, cmd: str, args: dict):
        if cmd == "Connected":
            self.checked_locations_local = set(args.get("checked_locations", []))
            self.print_available_tasks()
        elif cmd == "ReceivedItems":
            for item in args["items"]:
                name = self.item_names.lookup_in_game(item.item)
                logger.info(f"[Burglin' Gnomes] Received: {name}")
        elif cmd == "RoomUpdate":
            if "checked_locations" in args:
                self.checked_locations_local |= set(args["checked_locations"])

    def print_available_tasks(self):
        remaining = [
            t for t in ALL_TASK_NAMES
            if location_name_to_id[location_name_for_task(t)] not in self.checked_locations_local
        ]
        if not remaining:
            logger.info("All tasks completed!")
        else:
            logger.info("Remaining tasks:")
            for t in remaining:
                logger.info(f"  - {t}")

    def complete_task(self, task_name: str):
        loc_name = location_name_for_task(task_name)
        if loc_name not in location_name_to_id:
            logger.warning(f"Unknown task: '{task_name}'. Try /tasks to see valid names.")
            return
        loc_id = location_name_to_id[loc_name]
        if loc_id in self.checked_locations_local:
            logger.info("Already completed that one.")
            return
        asyncio.create_task(self.send_msgs([{"cmd": "LocationChecks", "locations": [loc_id]}]))
        self.checked_locations_local.add(loc_id)
        logger.info(f"Marked complete: {task_name}")


def launch():
    async def main(args):
        ctx = BurglinGnomesContext(args.connect, args.password)
        ctx.server_task = asyncio.create_task(server_loop(ctx), name="ServerLoop")
        if gui_enabled:
            ctx.run_gui()
        ctx.run_cli()
        await ctx.exit_event.wait()
        await ctx.shutdown()

    import colorama

    parser = get_base_parser()
    args = parser.parse_args()

    colorama.init()
    asyncio.run(main(args))
    colorama.deinit()


if __name__ == "__main__":
    launch()
