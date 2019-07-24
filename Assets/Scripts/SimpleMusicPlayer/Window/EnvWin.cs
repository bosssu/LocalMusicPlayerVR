using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EnvWin : WindowBase {

    Button btn_close;
    Transform tubeui;
    Text text_tube_image_name;
    Dropdown dropdown_lighttype;
    Slider slider_light_intensity;
    Slider slider_spotlight_angle;
    Slider slider_pointlight_range;
    Slider slider_abientlight_intensity;

    int tube_img_index;
    List<string> tube_images;

    string Tube_Image_Name
    {
        get { return Path.GetFileNameWithoutExtension(tube_images[tube_img_index]);  }
    }

    public override void Init()
    {
        base.Init();

        win_root = UIManager.Instance.UIMain.transform.Find("EnvPanel");

        btn_close = win_root.Find("btn_close").GetComponent<Button>();
        tubeui = win_root.Find("tubeui");
        dropdown_lighttype = win_root.Find("dropdown_lighttype").GetComponent<Dropdown>();
        slider_light_intensity = win_root.Find("slider_light_intensity").GetComponent<Slider>();
        slider_spotlight_angle = win_root.Find("slider_spotlight_angle").GetComponent<Slider>();
        slider_pointlight_range = win_root.Find("slider_pointlight_range").GetComponent<Slider>();
        slider_abientlight_intensity = win_root.Find("slider_abientlight_intensity").GetComponent<Slider>();

        tube_images = DataManager.Instance.Style_Data.tube_images;

        tubeui.Find("btn_last_tubeimg").GetComponent<Button>().onClick.AddListener(()=> {
            if (tube_images != null)
            {
                if (tube_img_index < 0) tube_img_index = tube_images.Count - 1;
                EnvManager.Instance. ChangeTubeTexture(tube_img_index);
                text_tube_image_name.text = Tube_Image_Name;
                DataManager.Instance.Data_Save.style.tube_image_name = tube_images[tube_img_index];
                DataManager.Instance.SaveData();
                tube_img_index--;
            }
        });

        tubeui.Find("btn_next_tubeimg").GetComponent<Button>().onClick.AddListener(() => {
            if (tube_images != null)
            {
                if (tube_img_index >= tube_images.Count) tube_img_index = 0;
                EnvManager.Instance.ChangeTubeTexture(tube_img_index);
                text_tube_image_name.text = Tube_Image_Name;
                DataManager.Instance.Data_Save.style.tube_image_name = tube_images[tube_img_index];
                DataManager.Instance.SaveData();
                tube_img_index++;
            }
        });

        btn_close.onClick.AddListener(() =>
        {
            this.Close();
        });

        text_tube_image_name = tubeui.Find("text_tube_image_name").GetComponent<Text>();
        text_tube_image_name.text = Path.GetFileNameWithoutExtension(EnvManager.Instance.tube_image_name);


        //禁用掉不必要的ui
        if (tube_images == null || tube_images.Count == 0)
            tubeui.gameObject.SetActive(false);

    }


}
