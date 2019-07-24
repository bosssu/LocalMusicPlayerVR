using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Collections;


public class Test : MonoBehaviour
{
    public MeshRenderer render;

    private void Start()
    {
        ReadAIPCFromMP3("D:/EXTERNAL_ASSETS/AUDIO/coop.mp3");
        //ReadMp3("D:/EXTERNAL_ASSETS/AUDIO/coop.mp3");
    }

    private int ReadAIPCFromMP3(string path)
    {
           FileStream fp = File.Open(path, FileMode.Open);
        if (fp == null)
        {
            Console.Write("cannot open this mp3\n");
            return -1;
        }

        // 这里的ID3V2以及帧标识的大小由标准规定均为10个字节
        // 这里其实应该是作为字节存储，用unsigned char更好，这里就简单用char替代吧
        byte[] cID3V2_head = new byte[10];
        byte[] cID3V2Fra_head = new byte[10];
        int ID3V2_len = 0;
        int lID3V2Fra_length = 0;
        byte[] cID3V2Fra = null;


        // 读取帧头，这里就是为了判断是否是ID3V2的标签头
        fp.Read(cID3V2_head, 0, 10);
        if ((cID3V2_head[0] == (byte)'I' || cID3V2_head[0] == (byte)'i') && (cID3V2_head[1] == (byte)'D' || cID3V2_head[1] == (byte)'d') && cID3V2_head[2] == (byte)'3')
        {
            // 获取ID3V2标签的长度
            Debug.Log(Encoding.Default.GetString(cID3V2_head));
            ID3V2_len = calcID3V2Len(cID3V2_head);
            Debug.Log(ID3V2_len);
        }

        bool hasAPIC = false;
        //这里来逐个读取帧标识，这里的专辑图片即保存在APIC标识里
        long offset = 0;
        while ((offset + 10) <= ID3V2_len)
        {
            // 这里每个帧标识的长度也为10，由于每个帧标识的存储的数据的长度不一
            // 每次要读取出来，进行运算获取真正数据长度
            fp.Read(cID3V2Fra_head, 0,10);
            lID3V2Fra_length = (int)(cID3V2Fra_head[4] * 0x100000000 + cID3V2Fra_head[5] * 0x10000 + cID3V2Fra_head[6] * 0x100 + cID3V2Fra_head[7]);
            Debug.Log(lID3V2Fra_length);

            // 这里判断是否为专辑图片的帧标识
            if (isFrameAPIC(cID3V2Fra_head))
            {
                hasAPIC = true;
                cID3V2Fra = new byte[lID3V2Fra_length];
                fp.Read(cID3V2Fra, 0, lID3V2Fra_length);
                break;
            }
            else
            {
                // 移动到下一帧标识
                offset = fp.Seek(lID3V2Fra_length, SeekOrigin.Current);
            }
        }

        fp.Close();

        // 到这里如果，专辑图片要么读出，要么就不存在
        if (hasAPIC)
        {
            // 这里整个数据的前面一部分数据是用来记录专辑图片的格式
            // 例如 image/jpeg image/png等，这里大部分的专辑图片都是jpeg的
            // 因此这里简单的只判断jpeg的格式，除去图片格式，数据前部依然有些是空数据
            // 因此以jpeg的标识来定位图片数据的起始
            int start = 0;
            byte[] temp = new byte[2];
            byte[] img_arr;
            while (start < lID3V2Fra_length - 1)
            {
                Array.Copy(cID3V2Fra, start, temp, 0, 2);
                if (isJPEG(temp))
                {
                    break;
                }
                ++start;
            }

            //int end = 0;
            //while (end < lID3V2Fra_length - 1)
            //{
            //    Array.Copy(cID3V2Fra, end, temp, 0, 2);
            //    if (isJPEGOVER(temp))
            //    {
            //        break;
            //    }
            //    ++end;
            //}

            // 是以JPEG格式存在，则存储为jpeg的文件
            if (start != lID3V2Fra_length - 1)
            {
                // 这里没有错误处理，从简
                img_arr = new byte[lID3V2Fra_length - start - 1];
                Array.Copy(cID3V2Fra, start, img_arr, 0, img_arr.Length);
                File.WriteAllBytes("D:/EXTERNAL_ASSETS/AUDIO/coop.jpg", img_arr);

                //Texture2D t2d = new Texture2D(640, 640);
                //t2d.LoadImage(img_arr);
                //t2d.Apply();

                //render.material.mainTexture = t2d;
                StartCoroutine(LoadTexture("E:/CloudMusic/Credits theme.jpg"));
            }
        }

        return 0;
    }

    private IEnumerator LoadTexture(string path)
    {
        WWW www = new WWW("file://" + path);
        yield return www;
        Debug.Log(www.error);
        render.material.mainTexture = www.texture;
    }


    private bool isFrameAPIC(byte[] cID3V2Fra_head)
    {
        byte[] b = new byte[4];
        Array.Copy(cID3V2Fra_head, b, 4);
        string str = Encoding.Default.GetString(b);
        Debug.Log(str);
        if (str == "APIC" || str == "apic")
        {
            return true;
        }
        return false;
    }

    private int calcID3V2Len(byte[] cID3V2_head)
    {
        int len = (cID3V2_head[6] & 0x7f) << 21 | (cID3V2_head[7] & 0x7f) << 14 | (cID3V2_head[8] & 0x7f) << 7 | (cID3V2_head[9] & 0x7f);
        return len;
    }

    private int calclID3V2FraLength(byte[] cID3V2Fra_head)
    {
        int len = (int)(cID3V2Fra_head[4] * 0x100000000 + cID3V2Fra_head[5] * 0x10000 + cID3V2Fra_head[6] * 0x100 + cID3V2Fra_head[7]);
        return len;
    }

    private bool isJPEG(byte[] data)
    {
        // JPEG格式数据的起始为0xFFD8，当然了，结束也有标志，但是这里不需要了
        if ((data[0] == 0x89) && (data[1] == 0x50))
        {
            return true;
        }
        return false;
    }

    private bool isJPEGOVER(byte[] data)
    {
        // JPEG格式数据的起始为0xFFD8，当然了，结束也有标志，但是这里不需要了
        if ((data[0] == 0x60) && (data[1] == 0x82))
        {
            return true;
        }
        return false;
    }

    public static string[] ReadMp3(string path)
    {
        int mp3TagID = 0;
        string[] tags = new string[6];
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        byte[] buffer = new byte[10];
        // fs.Read(buffer, 0, 128);
        string mp3ID = "";

        fs.Seek(0, SeekOrigin.Begin);
        fs.Read(buffer, 0, 10);
        int size = (buffer[6] & 0x7F) * 0x200000 + (buffer[7] & 0x7F) * 0x400 + (buffer[8] & 0x7F) * 0x80 + (buffer[9] & 0x7F);
        //int size = (buffer[6] & 0x7F) * 0X200000 * (buffer[7] & 0x7f) * 0x400 + (buffer[8] & 0x7F) * 0x80 + (buffer[9]);
        mp3ID = Encoding.Default.GetString(buffer, 0, 3);
        if (mp3ID.Equals("ID3", StringComparison.OrdinalIgnoreCase))
        {
            mp3TagID = 1;
            //如果有扩展标签头就跨过 10个字节
            if ((buffer[5] & 0x40) == 0x40)
            {
                fs.Seek(10, SeekOrigin.Current);
                size -= 10;
            }
            tags = ReadFrame(fs, size);
        }
        return tags;
    }
    public static string[] ReadFrame(FileStream fs, int size)
    {
        string[] ID3V2 = new string[6];
        byte[] buffer = new byte[10];
        while (size > 0)
        {
            //fs.Read(buffer, 0, 1);
            //if (buffer[0] == 0)
            //{
            //    size--;
            //    continue;
            //}
            //fs.Seek(-1, SeekOrigin.Current);
            //size++;
            //读取标签帧头的10个字节
            fs.Read(buffer, 0, 10);
            size -= 10;
            //得到标签帧ID
            string FramID = Encoding.Default.GetString(buffer, 0, 4);
            //计算标签帧大小，第一个字节代表帧的编码方式
            int frmSize = 0;

            frmSize = buffer[4] * 0x1000000 + buffer[5] * 0x10000 + buffer[6] * 0x100 + buffer[7];
            if (frmSize == 0)
            {
                //就说明真的没有信息了
                break;
            }
            //bFrame 用来保存帧的信息
            byte[] bFrame = new byte[frmSize];
            fs.Read(bFrame, 0, frmSize);
            size -= frmSize;
            string str = GetFrameInfoByEcoding(bFrame, bFrame[0], frmSize - 1);
            if (FramID.CompareTo("TIT2") == 0)
            {
                ID3V2[0] = "TIT2" + str;
            }
            else if (FramID.CompareTo("TPE1") == 0)
            {
                ID3V2[1] = "TPE1" + str;
            }
            else if (FramID.CompareTo("TALB") == 0)
            {
                ID3V2[2] = "TALB" + str;
            }
            else if (FramID.CompareTo("TIME") == 0)
            {
                ID3V2[3] = "TYER" + str;
            }
            else if (FramID.CompareTo("COMM") == 0)
            {
                ID3V2[4] = "COMM" + str;
            }
            else if (FramID.CompareTo("APIC") == 0)
            {
                Debug.Log("有图片信息");

                int i = 0;
                while (true)
                {

                    if (255 == bFrame[i] && 216 == bFrame[i + 1])
                    {
                        //在
                        break;

                    }
                    i++;
                }

                byte[] imge = new byte[frmSize - i];
                fs.Seek(-frmSize + i, SeekOrigin.Current);
                fs.Read(imge, 0, imge.Length);
                File.WriteAllBytes("D:/EXTERNAL_ASSETS/AUDIO/coop.jpeg", imge);
                Debug.Log("成功");
                //}
            }


        }
        return ID3V2;
    }
    public static string GetFrameInfoByEcoding(byte[] b, byte conde, int length)
    {
        string str = "";
        switch (conde)
        {
            case 0:
                str = Encoding.GetEncoding("ISO-8859-1").GetString(b, 1, length);
                break;
            case 1:
                //str = Encoding.GetEncoding("UTF-16LE").GetString(b, 1, length);
                break;
            case 2:
                str = Encoding.GetEncoding("UTF-16BE").GetString(b, 1, length);
                break;
            case 3:
                str = Encoding.UTF8.GetString(b, 1, length);
                break;
        }
        return str;
    }

}
