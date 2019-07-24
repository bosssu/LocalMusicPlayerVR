# -*- coding: utf-8 -*-

import mutagen
import os
import json
 
current_dir = os.getcwd()
fileNameList_all = os.listdir(current_dir)
fileNameList_shouldUse = []
failelist = []
success_amount = 0  #https://www.jianshu.com/p/53cf61220828 感谢这位老哥的帖子

#提取同目录下所有mp3的封面图和一些其他信息
def AudioFileAssetsExport(name):

    global success_amount
    json_savefile_name = str(name).replace('.mp3','')

    try:
        inf = mutagen.File(name)

        title = inf.tags['TIT2'].text[0]#标题
        author = inf.tags["TPE1"].text[0] #作者
        album = inf.tags["TALB"].text[0] #专辑

        #提取信息
        mp3info = {'title':title, 'author':author,'album':album}
        info_str = json.dumps(mp3info, sort_keys=True, indent=4, separators=(',', ': '))
        with open(json_savefile_name + '.json','w+') as mp3inf:
            mp3inf.write(info_str)

        #提取图片
        imgdata = inf.tags['APIC:'].data
        with open(json_savefile_name + '.jpg','wb') as img:
            img.write(imgdata)
        
        success_amount += 1

    except:
        failelist.append(name)

if(len(fileNameList_all) > 0):
    for filename in fileNameList_all:
        if(filename.endswith('.json' or filename.endswith('.jpg'))):
            os.remove(filename)

if(len(fileNameList_all)>0):
    for temp_name in fileNameList_all:
        if str(temp_name).endswith(".mp3"):
            fileNameList_shouldUse.append(temp_name)

# print('should process music file: ' + fileNameList_shouldUse)

if(len(fileNameList_shouldUse) > 0):
    for temp_name in fileNameList_shouldUse:
         AudioFileAssetsExport(temp_name)

print('------------------------------------------------------------------------------------------------------------------------------')
print('---------extract img success: ' + str(success_amount) + 'extract img fail: ' + str(len(fileNameList_shouldUse) - success_amount) + '----------------')
print(failelist)
os.system('pause')

