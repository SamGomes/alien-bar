import os
import json

with open('../../configs.cfg', 'r') as configsJson:
	gradeSelfWeight = json.load(configsJson)["GradeSelfWeight"]
	gradeInc = 0


	# my results
	myLearnerMetrics = "" 
	for filename in os.listdir(os.getcwd()):
		if(filename.__contains__(".txt") and not filename.__contains__(".meta")):
				with open(filename, 'r') as metricsJson:
					myLearnerMetrics = json.load(metricsJson)
					gradeInc = gradeInc + myLearnerMetrics["abilityInc"]*gradeSelfWeight


	# others results
	for filename in os.listdir(os.getcwd()+"/OthersResults"):
		if(filename.__contains__(".txt") and not filename.__contains__(".meta")):
			with open("./OthersResults/" + filename, 'r') as metricsJson:
				learnerMetrics = json.load(metricsJson)
				gradeInc = gradeInc + learnerMetrics["abilityInc"]*(1 - gradeSelfWeight)
	
	gradeInc = gradeInc / 3.0
	

	myLearnerMetrics["gradeInc"] = gradeInc
	with open('YourTaskResults.txt', 'w') as f:
		json.dump(myLearnerMetrics, f)

	print('YourTaskResults.txt generated at location: '+str(os.getcwd())+'! You can now upload your results to GIMME-Web!')