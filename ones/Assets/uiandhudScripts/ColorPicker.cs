using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ColorPicker : MonoBehaviour
{

    public Image previewImage;


    public int red;
    public int green;
    public int blue;

    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    public TextMeshProUGUI redText;
    public TextMeshProUGUI greenText;
    public TextMeshProUGUI blueText;


    // Start is called before the first frame update
    void Start()
    {
        updatePreviewImage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void updatePreviewImage(){

        previewImage.color = new Color(red/255f, green/255f, blue/255f, 1.0f);
    }



    public void redSliderChanged(){
        redText.text = redSlider.value.ToString();
        red = (int)redSlider.value;
        updatePreviewImage();
    }
    
    public void greenSliderChanged(){
        greenText.text = greenSlider.value.ToString();
        green = (int)greenSlider.value;
        updatePreviewImage();
    }
    
    public void blueSliderChanged(){
        blueText.text = blueSlider.value.ToString();
        blue = (int)blueSlider.value;
        updatePreviewImage();
    }

    public Color getColor(){
        return new Color(red/255f, green/255f, blue/255f, 1.0f);
    }

    public float getRed(){
        return red;
    }
    public float getGreen(){
        return green;
    }
    public float getBlue(){
        return blue;
    }

    public void setColor(float R, float G, float B){
        redSlider.value = R;
        greenSlider.value = G;
        blueSlider.value = B;
        redText.text = R.ToString();
        greenText.text = G.ToString();
        blueText.text = B.ToString();
    }

}
