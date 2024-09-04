.PHONY: build
build: 
	dotnet publish LargeFileSorting/LargeFileSortingApp/LargeFileSortingApp.csproj -o ./bin -c Release --framework net6.0
	dotnet publish LargeGenerateApp/LargeGenerateApp/LargeGenerateApp.csproj -o ./bin -c Release --framework net6.0	

.PHONY: cleanup
cleanup:
	rm -rf ./bin
	rm -rf ./data

.PHONY: run
run:
	$(eval INPUTFILE := $(word 2, $(MAKECMDGOALS)))
	$(eval OUTPUTFILE := $(word 3, $(MAKECMDGOALS)))
	dotnet publish LargeFileSorting/LargeFileSortingApp/LargeFileSortingApp.csproj -o ./bin -c Release --framework net6.0
	dotnet ./bin/LargeFileSortingApp.dll $(INPUTFILE) $(OUTPUTFILE)

.PHONY: run-debug
run-debug:
	$(eval INPUTFILE := $(word 2, $(MAKECMDGOALS)))
	$(eval OUTPUTFILE := $(word 3, $(MAKECMDGOALS)))
	time dotnet run --project LargeFileSorting/LargeFileSortingApp/ -- $(INPUTFILE) $(OUTPUTFILE)
	head -n 10 $(OUTPUTFILE)

.PHONY: gen
gen:
	$(eval CONFIGFILE := $(word 2, $(MAKECMDGOALS)))
	$(eval TESTFILE := $(word 3, $(MAKECMDGOALS)))
	dotnet publish LargeGenerateApp/LargeGenerateApp/LargeGenerateApp.csproj -o ./bin -c Release --framework net6.0
	dotnet ./bin/LargeGenerateApp.dll $(CONFIGFILE) $(TESTFILE)


TEST_PEFOMANCE_CONFIGS := ./PerfomanceTests/configs

.PHONY: test-perfomance
test-perfomance: build
	@for config_file in $(TEST_PEFOMANCE_CONFIGS)/*; do \
		echo "Test: $$config_file"; \
		test_file=./data/test; \
		output_file=./data/result.out; \
		dotnet ./bin/LargeGenerateApp.dll $$config_file $$test_file; \
		time dotnet ./bin/LargeFileSortingApp.dll $$test_file $$output_file; \
		rm -rf $$test_file; \
		rm -rf output_file; \
		echo "----------------------------------------------------------------------------------\n"; \
	done
	rm -rf ./data/*