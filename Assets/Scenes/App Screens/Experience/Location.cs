using Google.XR.ARCoreExtensions;
using UnityEngine;

[System.Serializable]
public class Location
{
    public double lat;
    public double lon;
    public GameObject firstAsset;

    public void setLat(double lat)
    {
        this.lat = lat;
    }

    public void setLon(double lon)
    {
        this.lon = lon;
    }

    public double getLat()
    {
        return lat;
    }

    public double getLong()
    {
        return lon;
    }
}
