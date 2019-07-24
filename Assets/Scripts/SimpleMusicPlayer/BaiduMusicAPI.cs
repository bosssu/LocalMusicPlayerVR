

using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class MMusic
{
    public string Title;
    public string Singer;
    public string PlayUrl;
}

public class MSongList
{
    public string pic_big;
    public string lrclink;
    public string song_id;
    public string title;

}

#region MQuerySong
public class MSongInfo
{
    public string songname;
    public string artistname;
    public string songid;
}

public class MQuerySong
{
    public List<MSongInfo> song;
    public string error_code;
}
#endregion

#region MPlaySong

public class MPlaySongInfo
{
    public string pic_small;
    public string author;
    public string song_id;
    public string title;
    public string lrclink;
}

public class MPlaySong
{
    public MPlaySongInfo songinfo;
    public string error_code;
}
#endregion


public class BaiduMusicAPI
{
    /// <summary>
    /// 搜索歌曲 解析网页的方式
    /// </summary>
    /// <param name="key"></param>
    //public static List<MMusic> Search(string key, int page)
    //{
    //    List<MMusic> musics = new List<MMusic>();

    //    if (page <= 0)
    //    {
    //        page = 1;
    //    }

    //    // http://music.baidu.com/search/song?s=1&key=%E5%87%89%E5%87%89&jump=0&start=0&size=20&third_type=0
    //    string url = string.Format("http://music.baidu.com/search/song?s=1&key={0}&jump=0&start={1}&size=20&third_type=0", key, (page - 1) * 20);

    //    MonoInstance.Instance.LoadPage(url, (str) =>
    //    {

    //        HtmlDocument doc = new HtmlDocument();
    //        doc.LoadHtml(html);
    //        HtmlNode node = doc.DocumentNode;
    //        HtmlNode resultdiv = node.SelectSingleNode("//div[@monkey='result-song']/div");
    //        HtmlNodeCollection list = resultdiv.SelectNodes("//div[@class='song-item clearfix yyr-song']");

    //        MMusic model = null;
    //        for (int i = 0; i < list.Count; i++)
    //        {
    //            model = new MMusic();
    //            HtmlNode temp = list[i];
    //            string title = temp.SelectSingleNode("//span[@class='song-title']/a/em").InnerText;
    //            string singer = temp.SelectSingleNode("//span[@class='singer']/span[@class='author_list']/a").GetAttributeValue("title", "");
    //            string playurl = temp.SelectSingleNode("//a[@class='list-micon icon-play']").GetAttributeValue("href", "");

    //            model.Title = title;
    //            model.Singer = singer;
    //            model.PlayUrl = playurl;
    //            musics.Add(model);
    //        }

    //        return musics;

    //    });

    //}

    //获取列表
    //参数：	type = 1-新歌榜,2-热歌榜,11-摇滚榜,12-爵士,16-流行,21-欧美金曲榜,22-经典老歌榜,23-情歌对唱榜,24-影视金曲榜,25-网络歌曲榜
    //size = 10 //返回条目数量
    // offset = 0 //获取偏移
    // http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.billboard.billList&type=1&size=10&offset=0

    /// <summary>
    /// 获取列表 type = 1-新歌榜,2-热歌榜....
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static void GetSongList(int type, int size, int offset,Action<MSongList> callback)
    {
        MSongList list = new MSongList();
        string url = string.Format("http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.billboard.billList&type={0}&size={1}&offset={2}", type, size, offset);
        MonoInstance.Instance.LoadPage(url, (content) => {
            list = JsonConvert.DeserializeObject<MSongList>(content);
            if (callback != null) callback(list);
        });
    }

    // 搜索
    // 参数：query = '' //搜索关键字
    // http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.search.catalogSug&query=%E4%B8%80%E5%89%91%E8%BD%BB%E5%AE%89

    /// <summary>
    /// 搜索歌名或歌手
    /// </summary>
    /// <param name="key"></param>
    public static void QuerySong(string key,Action<MQuerySong> callback)
    {
        MQuerySong model = new MQuerySong();
        string url = string.Format("http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.search.catalogSug&query={0}", key);
        MonoInstance.Instance.LoadPage(url, (content) => {
            content = content.Replace("(", string.Empty).Replace(");", string.Empty);
            model = JsonConvert.DeserializeObject<MQuerySong>(content);
            if (callback != null) callback(model);
        });
    }

    // 播放
    // 参数：songid = 877578 //歌曲id
    // http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.song.play&songid=877578

    /// <summary>
    /// 播放歌曲信息
    /// </summary>
    /// <param name="songid"></param>
    /// <returns></returns>
    public static void PlaySong(string songid,Action<MPlaySong> callback)
    {
        MPlaySong model = new MPlaySong();
        string url = string.Format("http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.song.play&songid={0}", songid);
        MonoInstance.Instance.LoadPage(url, (content) => {
            content = content.Replace("(", string.Empty).Replace(");", string.Empty);
            model = JsonConvert.DeserializeObject<MPlaySong>(content);
            if (callback != null) callback(model);
        });
    }

    // 歌词
    // 参数：songid = 877578 //歌曲id
    // http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.song.lry&songid=877578

    /// <summary>
    /// 歌词信息
    /// </summary>
    /// <param name="songid"></param>
    /// <returns></returns>
    //public static MPlaySongLrc PlaySongLRC(string songid)
    //{
    //    MPlaySongLrc model = new MPlaySongLrc();
    //    string url = string.Format("http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.song.lry&songid={0}", songid);
    //    MyHttpClient client = new MyHttpClient(url);
    //    string html = client.ExecuteGet();
    //    model = JsonConvert.DeserializeObject<MPlaySongLrc>(html);

    //    return model;
    //}


    // 下载
    // 参数：	songid = 877578//歌曲id
    // _t = 1430215999,, //时间戳
    // http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.song.downWeb&songid=73866069&bit=24&_t=1492565440

    /// <summary>
    /// 下载歌曲到本地 不可用
    /// </summary>
    /// <param name="songid"></param>
    /// <returns></returns>
    //public static string DownLoadSong(string songid, string filepath)
    //{
    //    string _t = StringUtils.GetTimeStamp(true);
    //    string url = string.Format("http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.song.downWeb&songid={0}&bit=24&_t={1}", songid, _t);

    //    MyHttpClient client = new MyHttpClient(url, filepath, true);
    //    string path = client.HttpDownloadFile();

    //    return path;
    //}

    /// <summary>
    /// 通过播放信息中的url去下载
    /// </summary>
    /// <param name="url"></param>
    /// <param name="filepath"></param>
    /// <param name="api"></param>
    /// <returns></returns>
    //public static string DownLoadSong(string url, string filepath, bool api)
    //{
    //    MyHttpClient client = new MyHttpClient(url, filepath, true);
    //    string path = client.HttpDownloadFile();

    //    return path;
    //}


    // 歌手信息
    // 参数：	tinguid = 877578 //歌手ting id
    // http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.artist.getInfo&tinguid=877578

    /// <summary>
    /// 歌手信息
    /// </summary>
    /// <param name="ting_id"></param>
    /// <returns></returns>
    //public static MArtistInfo GetArtistInfo(string ting_id)
    //{
    //    MArtistInfo model = new MArtistInfo();
    //    string url = string.Format("http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.artist.getInfo&tinguid={0}", ting_id);
    //    MyHttpClient client = new MyHttpClient(url);
    //    string json = client.ExecuteGet();

    //    json = json.Replace("(", string.Empty).Replace(");", string.Empty);

    //    model = JsonConvert.DeserializeObject<MArtistInfo>(json);

    //    return model;
    //}

    // 歌手歌曲列表
    // 参数：	tinguid = 877578//歌手ting id
    // limits = 6//返回条目数量
    // 
    // http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.artist.getSongList&tinguid=110942340&limits=20&use_cluster=1&order=2

    /// <summary>
    /// 获取歌手歌曲列表
    /// </summary>
    /// <param name="tinguid"></param>
    /// <param name="limits"></param>
    /// <returns></returns>
    //public static MSingerSongList GetSingerSongs(string tinguid, int limits)
    //{
    //    MSingerSongList model = new MSingerSongList();
    //    string url = string.Format("http://tingapi.ting.baidu.com/v1/restserver/ting?format=json&callback=&from=webapp_music&method=baidu.ting.artist.getSongList&tinguid={0}&limits={1}&use_cluster=1&order=2", tinguid, limits);

    //    return model;
    //}
}