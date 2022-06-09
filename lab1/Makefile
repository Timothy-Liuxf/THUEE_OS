################################################################################
#
#  This file is part of the THUEE_OS project.
#
#  Copyright (C) 2022 Timothy-LiuXuefeng
#
#  MIT License
#
################################################################################

.PHONY: build debug run test retest clean

NTELLER ?= 3

build:
	$(MAKE) -C src

debug: build
	./src/target/main $(NTELLER)

test:
	$(MAKE) -C test

retest:
	$(MAKE) -C test clean && $(MAKE) -C test

run: build test
	./src/target/main $(NTELLER) < ./test/test.txt

clean:
	$(MAKE) -C src clean
	$(MAKE) -C test clean
