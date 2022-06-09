################################################################################
#
#  This file is part of the THUEE_OS project.
#
#  Copyright (C) 2022 Timothy-LiuXuefeng
#
#  MIT License
#
################################################################################

export PYTHON ?= python3

NCUSTOMER ?= 10
MINSERVTIME ?= 1
MAXSERVTIME ?= 10
MINENTERTIME ?= 1
MAXENTERTIME ?= 10

test.txt: test.py
	$(PYTHON) test.py \
	--ncustomer=$(NCUSTOMER) \
	--minservtime=$(MINSERVTIME) \
	--maxservtime=$(MAXSERVTIME) \
	--minentertime=$(MINENTERTIME) \
	--maxentertime=$(MAXENTERTIME) \
	$(NOTHING)

.PHONY: clean
clean:
	-$(RM) test.txt
