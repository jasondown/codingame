// https://www.codingame.com/training/easy/defibrillators

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static void Main(string[] args)
    {
        string lon = Console.ReadLine();
        string lat = Console.ReadLine();
        int n = int.Parse(Console.ReadLine());

        var defibs = new Defib[n];

        for (int i = 0; i < n; i++)
        {
            var defibDetails = Console.ReadLine().Split(';');
            defibs[i] = new Defib
            {
                ID = Int32.Parse(defibDetails[0]),
                Name = defibDetails[1],
                Address = defibDetails[2],
                Phone = defibDetails[3],
                Longitude = Double.Parse(defibDetails[4].Replace(',', '.')),
                Latitude = Double.Parse(defibDetails[5].Replace(',', '.'))
            };
        }

        Console.WriteLine(FindClosestDefib(lon, lat, defibs));
    }

    static string FindClosestDefib(string lon, string lat, IEnumerable<Defib> defibs)
    {
        const double earthRadius = 6371;
        var longitude = Double.Parse(lon.Replace(',', '.'));
        var latitude = Double.Parse(lat.Replace(',', '.'));

        double shortestDistance = Double.MaxValue;
        Defib closestDefib = null;

        foreach (var d in defibs)
        {
            var x = (longitude - d.Longitude) * Math.Cos((d.Latitude + latitude) / 2);
            var y = latitude - d.Latitude;
            var dist = Math.Sqrt((x * x) + (y * y)) * earthRadius;

            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                closestDefib = d;
            }
        }

        return closestDefib.Name;
    }
}

public class Defib
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
}