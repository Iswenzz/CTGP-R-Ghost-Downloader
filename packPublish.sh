#!/bin/bash
arr=("linux-x64" "osx-x64" "win-x64")
name=ctgp-r-ghost-downloader

cd "./bin/Release/netcoreapp3.1/publish"
for t in ${arr[@]}; do
	zip -9 -x "*.pdb" -r $name-$t.zip ./$t
done