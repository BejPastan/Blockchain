import os

def cleanDir(path):
    try:
        entries = os.listdir(path)
    except:
        print(path, " was not a dir")
        return
    
    for entry in entries:
        if ".meta" in entry:
            
            os.remove(path+"/"+entry)
            continue
        if "." in entry:
            continue
        else:
            cleanDir(path+"/"+entry)

cleanDir(".")