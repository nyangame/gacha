using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Manager : MonoBehaviour
{
    [SerializeField] NotionSetting _setting;
    [SerializeField] RectTransform _box;
    [SerializeField] GameObject _userPrefab;
    [SerializeField] Text _hitName;
    [SerializeField] Text _count;
    [SerializeField] UnityEngine.UI.Image _rare;
    [SerializeField] Color[] _rareColor = new Color[3];
    [SerializeField] UnityEngine.UI.Button _gacha;
    [SerializeField] UnityEngine.UI.Button _next;

    [Serializable]
    public class Notion
    {
        public string child_id;
        public string page_type;
        public string url;
        public bool Check;
        public string Name;
    }

    [Serializable]
    public class Response
    {
        public Notion[] Data;
    }

    [Serializable]
    class PostBody
    {
        public string token;
    }

    class Human
    {
        public string Name;
        public bool Check;
        public int Rare;
        public int Rate;
        public GameObject Button;
    }

    List<Human> _humans = new List<Human>();
    List<Human> _stock = new List<Human>();
    List<Human> _nextStock = new List<Human>();

    private void Start()
    {
        LoadHuman();
    }

    //マスタデータ読み込み関数
    private void LoadHuman()
    {
        Debug.Log("request");
        Network.WebRequest.PostRequest(_setting.URI, new PostBody() { token = _setting.Token }, (string json) =>
        {
            Debug.Log("{ \"Data\": " + json + " }");
            var res = JsonUtility.FromJson<Response>("{ \"Data\" : " + json + " }");
            foreach (var d in res.Data)
            {
                _humans.Add(new Human() { Name = d.Name, Check = d.Check });
            }
            _gacha.enabled = true;
            ResetGacha();
        });
    }

    void ResetGacha()
    {
        foreach (var h in _humans)
        {
            if (h.Check == false) continue;

            h.Rate = UnityEngine.Random.Range(1, 10);
            if (h.Rate < 3) h.Rare = 2;
            else if (h.Rate < 7) h.Rare = 1;
            else h.Rare = 0;

            h.Button = GameObject.Instantiate(_userPrefab, _box.transform);
            h.Button.GetComponentInChildren<Text>().text = h.Name;
            h.Button.GetComponentInChildren<UnityEngine.UI.Image>().color = _rareColor[h.Rare];

            _stock.Add(h);
        }
        Debug.Log(_stock.Count);
        _count.text = _stock.Count + "人";
        _box.sizeDelta = new Vector2(260, 54 * _stock.Count);
    }

    public void DoGacha()
    {
        if (_stock.Count <= 0) ResetGacha();

        int total = _stock.Sum(s => s.Rate);
        int rand = UnityEngine.Random.Range(0, total);

        Human tgt = null;
        foreach (var s in _stock)
        {
            if(rand < s.Rate)
            {
                tgt = s;
                break;
            }

            rand -= s.Rate;
        }

        Destroy(tgt.Button);

        _hitName.text = tgt.Name;
        _rare.color = _rareColor[tgt.Rare];

        _stock.Remove(tgt);

        if (_stock.Count <= 0) ResetGacha();

        _box.sizeDelta = new Vector2(260, 54 * _stock.Count);
        _count.text = _stock.Count + "人";
    }
}
