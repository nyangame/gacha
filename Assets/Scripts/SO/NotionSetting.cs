using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NotionSetting", menuName = "Gacha/NotionSetting", order = 1)]
public class NotionSetting : ScriptableObject
{
    [SerializeField] string _token;
    [SerializeField] string _databaseId;
    [SerializeField] string _uri;

    public string Token => _token != null ? _token : "";
    public string DatabaseID => _databaseId != null ? _databaseId : "";
    public string URI => _uri != null ? _uri : "";
}
