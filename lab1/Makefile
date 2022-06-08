.PHONY: build debug run test retest clean

build:
	$(MAKE) -C src

debug: build
	./src/target/main

test:
	$(MAKE) -C test

retest:
	$(MAKE) -C test clean && $(MAKE) -C test

run: build test
	./src/target/main < ./test/test.txt

clean:
	$(MAKE) -C src clean
	$(MAKE) -C test clean
