GEN_PROJECT_DIR="./../LargeGenerateApp/LargeGenerateApp"
APP_PROJECT_DIR="./../LargeFileSorting/LargeFileSortingApp"

CONFIG_DIR="./configs"

if [ ! -d "$CONFIG_DIR" ]; then
  echo "Directory with config files $CONFIG_DIR does not exist."
  exit 1
fi

for config_file in "$CONFIG_DIR"/*.json; 
do
  if [[ -f "$config_file" ]]; then
    echo "Processing file: $config_file"
    
    dotnet run --project "$GEN_PROJECT_DIR" -- "$config_file" test.txt

    time dotnet run --project "$APP_PROJECT_DIR" -- test.txt

    if [ $? -ne 0 ]; then
      echo "Error processing file: $config_file"
    else
      echo "Successfully processed file: $config_file"
    fi
  else
    echo "No .json files found in directory $CONFIG_DIR"
    exit 1
  fi

  echo "\n"
done

echo "All files processed."
