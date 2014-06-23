all: test

test: test.c
	g++ -Wall -g -o test test.c `pkg-config --cflags --libs /usr/mymono/lib/pkgconfig/monosgen-2.pc`

clean:
	rm -f test

