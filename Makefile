.PHONY: run gen run-debug

run:
	$(eval INPUTFILE := $(word 2, $(MAKECMDGOALS)))
	$(eval OUTPUTFILE := $(word 3, $(MAKECMDGOALS)))
	dotnet publish LargeFileSorting/LargeFileSortingApp/LargeFileSortingApp.csproj -o ./bin -c Release --framework net6.0
	dotnet ./bin/LargeFileSortingApp.dll $(INPUTFILE) $(OUTPUTFILE)

run-debug:
	$(eval INPUTFILE := $(word 2, $(MAKECMDGOALS)))
	$(eval OUTPUTFILE := $(word 3, $(MAKECMDGOALS)))
	time dotnet run --project LargeFileSorting/LargeFileSortingApp/ -- $(INPUTFILE) $(OUTPUTFILE)
	head -n 10 $(OUTPUTFILE)

gen:
	$(eval CONFIGFILE := $(word 2, $(MAKECMDGOALS)))
	$(eval TESTFILE := $(word 3, $(MAKECMDGOALS)))
	dotnet publish LargeGenerateApp/LargeGenerateApp/LargeGenerateApp.csproj -o ./bin -c Release --framework net6.0
	dotnet ./bin/LargeGenerateApp.dll $(CONFIGFILE) $(TESTFILE)

# TODO: create-gen-config, build, run-debug, test-perfomance, may be test