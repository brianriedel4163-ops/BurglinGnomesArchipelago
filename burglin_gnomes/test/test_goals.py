from . import BurglinGnomesTestBase


class TestDefault(BurglinGnomesTestBase):
    options = {"tasks_required": 25}


class TestMinTasks(BurglinGnomesTestBase):
    options = {"tasks_required": 5}


class TestAllTasks(BurglinGnomesTestBase):
    options = {"tasks_required": 50}
