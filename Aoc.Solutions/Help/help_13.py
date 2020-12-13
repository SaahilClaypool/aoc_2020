# https: // github.com/Fiddle-N/advent-of-code-2020/blob/master/day_13/process.py
import math
import timeit

import sympy.ntheory.modular


class ShuttleSearch:

    def __init__(self, bus_input=None):
        bus_input = bus_input if bus_input is not None else self._read_file()
        raw_timestamp, raw_bus_ids = bus_input.splitlines()
        self.timestamp = int(raw_timestamp)
        self.bus_ids = []
        for raw_bus_id in raw_bus_ids.split(','):
            try:
                bus_id = int(raw_bus_id)
            except ValueError:
                if raw_bus_id != 'x':
                    raise
                bus_id = raw_bus_id
            self.bus_ids.append(bus_id)

    @staticmethod
    def _read_file():
        with open('input.txt') as f:
            return f.read()

    def earliest_bus(self):
        valid_bus_ids = [bus_id for bus_id in self.bus_ids if bus_id != 'x']

        def waited_mins(id):
            return id - (self.timestamp % id)

        bus_id_waited_mins = {bus_id: waited_mins(
            bus_id) for bus_id in valid_bus_ids}
        earliest_bus = min(bus_id_waited_mins.items(), key=lambda x: x[1])
        return earliest_bus

    def _mod_inv(self, bus_id_prod, bus_id):
        return pow(bus_id_prod, -1, bus_id)

    def chinese_remainder(self, use_sympy=False):
        bus_id_mod_result = {}
        for pos, bus_id in enumerate(self.bus_ids):
            if bus_id == 'x':
                continue
            bus_id_mod_result[bus_id] = (bus_id - pos) % bus_id
        if use_sympy:
            result, _ = sympy.ntheory.modular.crt(
                bus_id_mod_result.keys(), bus_id_mod_result.values())
            return result
        else:
            if math.gcd(*bus_id_mod_result.keys()) != 1:
                raise Exception(
                    'bus ids are not coprime, chinese remainder not possible')
            total_prod = math.prod(bus_id_mod_result.keys())
            result = 0
            for bus_id, mod_result in bus_id_mod_result.items():
                bus_id_prod = total_prod // bus_id
                mod_inv = self._mod_inv(bus_id_prod, bus_id)
                bus_id_result = mod_result * mod_inv * bus_id_prod
                result += bus_id_result
            return result % total_prod


def main():
    shuttle_search = ShuttleSearch()
    print(
        f'ID of earliest bus multiplied by number of minutes: {math.prod(shuttle_search.earliest_bus())}')
    print(
        f'Earliest timestamp that bus IDs depart at offsets matching their positions: {shuttle_search.chinese_remainder()}')


if __name__ == '__main__':
    print(f'Completed in {timeit.timeit(main, number=1)} seconds')
