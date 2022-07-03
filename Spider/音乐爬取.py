import requests
import re
import os
import time
import parsel
import json
# 保存数据模块
def get_data(a, file_name, count):
   try:
      with open('./' + image_map + '/' + file_name+".csv"   , "a") as f:
         f.write(a)
         f.write('\n')
         print('第%d张数据' % count)
         print("保存完毕")
   except:
      print('==========' + '下载失败================')
      print(file_name + "下载失败")
# 要访问的网址
url ='https://music.163.com/discover/artist/cat?id=1001'
#创建保存资源目录
image_map ="音乐文本"
if not os.path.exists(image_map):
   os.mkdir(image_map)

headers = {'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36'}
r = requests.get(url= url,headers=headers)#获取网页的对象
html =r.text #网页数据
selector = parsel.Selector(html)
data = selector.xpath('//div[@class="u-cover u-cover-5"]/a/@href').getall()
# 进入歌手的网页，获取指定的歌曲数据
for image_html in data:
   image_html_data = requests.get(url="https://music.163.com/"+image_html,headers=headers).text
   print(image_html_data)
   selector = parsel.Selector(image_html_data)
   # 获取到指定的属性/数据：歌名，歌单，时长，歌手，歌曲图片链接
   s_name =selector.xpath('//ul[@class="f-hide"]/li/a/text()').getall();
   ID = selector.xpath('//ul[@class="f-hide"]/li/a/@href').getall()
   keyStr="\d+"
   ID=re.findall(keyStr,str(ID))
   # 歌手
   singer=selector.xpath('//div[@class="btm"]/h2/text()').getall();
   # 时长
   key_str='"duration":(.*?),'
   duration=re.findall(key_str,selector.xpath('//textarea[@id="song-list-pre-data"]/text()').getall()[0])
   # 专辑名
   album =selector.xpath('//textarea[@id="song-list-pre-data"]/text()').getall();
   key_str2='"album":{"id":\d+,"name":(.*?),'
   album=re.findall(key_str2,album[0])
   # 歌曲图片链接
   img_link=selector.xpath('//textarea[@id="song-list-pre-data"]/text()').getall();
   key_str3 = '"picUrl":"(.*?)","'
   img_link=re.findall(key_str3,img_link[0])

   # 求出全部数据中的最少数量（确保保存到的数据都非空）
   list=[len(s_name),len(ID),len(duration),len(album),len(img_link)]
   minlen=min(list)
   # 保存数据
   for i in range(minlen):
      time.sleep(1)
      file_name = singer[0]
      # finaldata=str(s_name[i]+'*'+ID[i]+'*'+singer[0]+'*'+duration[i]+'*'+album[i]+'*'+img_link[i])
      finaldata =str(album[i])
      print(finaldata)

      get_data(finaldata,file_name,i)


