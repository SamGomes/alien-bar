import json

with open('../../configs.cfg', 'r') as configsJson:
	print(configsJson)
	
	gradeSelfWeight = json.load(configsJson)
	for key, value in gradeSelfWeight.items():
		print(key, ":", value)

# with open("s2.txt", "r") as read_file:
#     print(json.load(read_file).abilityInc)


# for filename in os.listdir(directory):
#     f = os.path.join(directory, filename)
#     # checking if it is a file
#     if os.path.isfile(f):
#         print(f)