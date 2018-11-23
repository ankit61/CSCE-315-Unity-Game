using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TextFunctions {

    public static IEnumerator FadeTextToFullAlpha(float _f, Text _t)
    {
        _t.color = new Color(_t.color.r, _t.color.g, _t.color.b, 0);
        while (_t.color.a < 1.0f)
        {
            _t.color = new Color(_t.color.r, _t.color.g, _t.color.b, _t.color.a + (Time.deltaTime / _f));
            yield return null;
        }
    }

    public static IEnumerator FadeTextToZeroAlpha(float _f, Text _t)
    {
        _t.color = new Color(_t.color.r, _t.color.g, _t.color.b, 1);
        while (_t.color.a > 0.0f)
        {
            _t.color = new Color(_t.color.r, _t.color.g, _t.color.b, _t.color.a - (Time.deltaTime / _f));
            yield return null;
        }
    }

}
