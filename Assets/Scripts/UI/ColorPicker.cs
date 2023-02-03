using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace gameracers.UI
{
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField] private Color colour;
        [SerializeField] string red = "Red";
        [SerializeField] string green = "Green";
        [SerializeField] string blue = "Blue";
        [SerializeField] string alpha = "Alpha";
        [SerializeField] GameObject cursor;

        [SerializeField] Texture2D texture;
        RectTransform trans;
        Vector2 imageOffset;

        private void OnEnable()
        {
            EventListener.onSliderChange += SetColour;
        }

        private void OnDisable()
        {
            EventListener.onSliderChange -= SetColour;
        }

        private void Start()
        {
            colour = Color.black;
            EventListener.ColourChange(colour);
            trans = GetComponent<RectTransform>();
            imageOffset = new Vector2(
                (trans.position.x) - (trans.rect.width / 2f * trans.localScale.x),
                (trans.position.y) - (trans.rect.height / 2f * trans.localScale.y));
        }

        private void SetColour(string origin, float val)
        {
            if (origin == red)
                colour = new Color(val, colour.g, colour.b, colour.a);
            if (origin == green)
                colour = new Color(colour.r, val, colour.b, colour.a);
            if (origin == blue)
                colour = new Color(colour.r, colour.g, val, colour.a);
            if (origin == alpha)
                colour = new Color(colour.r, colour.g, colour.b, val);
            cursor.GetComponent<Image>().color = colour;
            EventListener.ColourChange(colour);
        }

        public void GetColour()
        {
            Vector2 mousePos = new Vector2((Input.mousePosition.x - imageOffset.x) / trans.localScale.x, 
                (Input.mousePosition.y - imageOffset.y) / trans.localScale.y);
            colour = texture.GetPixel((int)mousePos.x, (int)mousePos.y);
            EventListener.ColourChange(colour);
            cursor.GetComponent<Image>().color = colour;
            cursor.GetComponent<RectTransform>().position = Input.mousePosition;
            //Debug.Log(trans.rect.width);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if ((Input.mousePosition.x - imageOffset.x) / trans.localScale.x < 0 || 
                    (Input.mousePosition.x - imageOffset.x) / trans.localScale.x > trans.rect.width) return;
                if ((Input.mousePosition.y - imageOffset.y) / trans.localScale.y < 0 ||
                    (Input.mousePosition.y - imageOffset.y) / trans.localScale.y > trans.rect.height) return;
                GetColour();
            }
        }

    }
}
