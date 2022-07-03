import requests
import re
import xlrd
import csv
import json
import time

data = {"h3R3","陈奕迅","队长","李荣浩","林俊杰",
        "毛不易","许嵩","薛之谦","颜人中","周杰伦"}
def postAlbums(url):
    # 上传专辑
    ArtistId=0;
    for i in data:
        print(i)
        with open("D:/编程/pycharm/练习文件/venv/新建文件夹/"+i+".csv") as f:
          reader = csv.DictReader(f)
          for row in reader:#获取到对应的行的对应属性
              alumId =int(row['AlumId'])
              alum=row['Alum']
              # 以json形式暂时存储数据
              data = {"Id": int(alumId), "name": alum}
              jone = json.dumps(data)
              print(jone)
              html_post = requests.post(url, json={"Id":int(alumId),"name":alum})
              print(html_post.request.body)
              print(html_post.status_code)
              print(html_post.reason)
              time.sleep(0.1)
def postSongs(url):
        #         上传歌曲
    ArtistId=0;
    for i in data:
        print(i)
        with open("D:/编程/pycharm/练习文件/venv/新建文件夹/"+i+".csv") as f:
          reader = csv.DictReader(f)
          for row in reader:#获取到对应的行的对应属性
            # 以json形式暂时存储数据
            data={
                "Id": int(row['Id']),
                "Count":0,
                "Title":row['Name'],
                "ArtistId":[int(row['ArtistId'])],
                "Duration":row['Duration'],
                "AlbumId":int(row['AlumId']) ,
                "ImgLink":(row['ImgLink']),
             }
            # 上传数据到数据库
            html_post = requests.post(url, json=data)
            print(html_post.status_code)
            print(html_post.reason)
            time.sleep(0.1)

def postArtists(url):
    # 上传Artists

    data = {"h3R3", "陈奕迅", "队长", "李荣浩", "林俊杰",
          "毛不易", "许嵩", "薛之谦", "颜人中", "周杰伦"}
    ArtistId = 0;
    for i in data:
      print(i)
      with open("D:/编程/pycharm/练习文件/venv/新建文件夹/" + i + ".csv") as f:
          reader = csv.DictReader(f)
          for row in reader:  # 获取到对应的行的对应属性
              ArtistId =int(row['ArtistId'])
          print(ArtistId)
          # 以json形式暂时存储数据
          html_post = requests.post(url, json={"Id":int(ArtistId),"name":i})
          print(html_post.request.body)
          print(html_post.status_code)
          print(html_post.reason)
          time.sleep(0.1)

urlAlbums = "https://emocloud.azurewebsites.net/api/Albums"
urlArtists ="https://emocloud.azurewebsites.net/api/Artists"
urlSongs="https://emocloud.azurewebsites.net/api/Songs"
# 上传数据
postAlbums(urlAlbums)
postSongs(urlSongs)
postArtists(urlArtists)
