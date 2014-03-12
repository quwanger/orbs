# Make sure that the GOPATH directory is correct for your setup

# PIIP=172.17.156.148
PIIP=192.168.1.233
PIDIR=/home/pi/orbs/
.PHONY: all test clean

MAKEFILE_OPTIONS=-s

all:
	make test
	make goserver

test: test/main.go
	export PROJECT=test; \
	export TARGET=main; \
	make ${MAKEFILE_OPTIONS} build

pi-test: test/main.go
	export GOARCH=arm; \
	export GOOS=linux; \
	export GOARM=5; \
	make ${MAKEFILE_OPTIONS} test

goserver: GoServer/src/main.go
	export PROJECT=GoServer/src; \
	export TARGET=main; \
	export GOPATH=${HOME}/orbs/GoServer; \
	make ${MAKEFILE_OPTIONS} build

pi-goserver: GoServer/src/main.go
	export GOARCH=arm; \
	export GOOS=linux; \
	export GOARM=5; \
	make ${MAKEFILE_OPTIONS} goserver

orbsserver: OrbsServer/src/main.go
	export PROJECT=OrbsServer/src; \
	export TARGET=main; \
	export GOPATH=${HOME}/orbs/OrbsServer; \
	make ${MAKEFILE_OPTIONS} build

pi-orbsserver: OrbsServer/src/main.go
	export GOARCH=arm; \
	export GOOS=linux; \
	export GOARM=5; \
	make ${MAKEFILE_OPTIONS} goserver

build:
	export TARGETPATH=$(PROJECT)/$(TARGET); \
	echo ""; \
	echo "---> BUILDING - $$TARGETPATH.go"; \
	echo ""; \
	go build -o $$TARGETPATH $$TARGETPATH.go \
	&& make ${MAKEFILE_OPTIONS} send_pi

send_pi:
	# Check if the ARM/ARCH variables are set.
	if [ -n "$(GOARCH)" ]; then \
		echo ""; \
		echo "---> SENDING $(TARGETPATH) TO PI"; \
		echo ""; \
		scp $(TARGETPATH) pi@$(PIIP):$(PIDIR)bin; \
	fi

clean:
	rm test/main