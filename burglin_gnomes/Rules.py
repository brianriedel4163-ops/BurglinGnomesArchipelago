from .Items import PAYOUT_ITEM


def set_rules(world) -> None:
    """No entrance rules (see Regions.py -- everything's flat/reachable).
    Goal: collect enough "Gnomium Payout" copies, one of which is placed at
    exactly `tasks_required` of the 50 task locations (see __init__.create_items).
    """
    multiworld = world.multiworld
    player = world.player
    required = world.options.tasks_required.value

    multiworld.completion_condition[player] = (
        lambda state: state.has(PAYOUT_ITEM, player, required)
    )
