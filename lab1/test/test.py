################################################################################
#
#  This file is part of the THUEE_OS project.
#
#  Copyright (C) 2022 Timothy-LiuXuefeng
#
#  MIT License
#
################################################################################

import argparse
from random import randint

if __name__ == '__main__':
    parser = argparse.ArgumentParser('Generate test examples.')
    parser.add_argument('--ncustomer', required=False, default=10, type=int, help='Number of customers')
    parser.add_argument('--minservtime', required=False, default=1, type=int)
    parser.add_argument('--maxservtime', required=False, default=10, type=int)
    parser.add_argument('--minentertime', required=False, default=1, type=int)
    parser.add_argument('--maxentertime', required=False, default=10, type=int)
    args = parser.parse_args()
    nCustomer = args.ncustomer
    minServTime = args.minservtime
    maxServTime = args.maxservtime
    minEnterTime = args.minentertime
    maxEnterTime = args.maxentertime
    assert(minServTime <= maxServTime)
    assert(minEnterTime <= maxEnterTime)
    lines = []
    for i in range(nCustomer):
        lines.append(f'{i} {randint(minEnterTime, maxEnterTime)} {randint(minServTime, maxServTime)}\n')
    with open('test.txt', 'w', encoding='utf-8') as f:
        f.writelines(lines)
