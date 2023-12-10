using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SklizeniPomerancu
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var item = PripravaVstupu();
            Stromek stromek = new Stromek();
            stromek.SestavGraf(item.Item1, item.pocetVrcholu);
            foreach(int n in item.pomerančeKeSklizeni)
            {
                Console.WriteLine("return "+stromek.SklidPomerance(n));
            }
            Console.ReadLine();
        }
        static (List<(int, int , int)> , int pocetVrcholu, List<int> pomerančeKeSklizeni) PripravaVstupu()
        {
            List<(int Vrchol1, int Vrchol2, int PocetPomerancu)> values = new List<(int, int, int)>();
            //Vrchol 1, Vrchol2, Počet Pomerančů
            List<string> input = new List<string>();
            using (StreamReader sr = new StreamReader(@"C:\Users\macou\source\repos\SklizeniPomerancu\SklizeniPomerancu\input.txt"))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    input.Add(s);
                }
            }
            int index = 1;
            for (int i = 2; i < int.Parse(input[1])+2; i++)
            {
                index++;
                string lajn = input[i];
                String[] line = lajn.Split(' ');
                int vrchol1 = int.Parse(line[0]);
                int vrchol2 = int.Parse(line[1]);
                int pocetPomarancu = int.Parse(line[2]);
                values.Add((vrchol1, vrchol2, pocetPomarancu));
            }
            index++;
            int početPomerančů = int.Parse(input[index]);
            index++;
            List<int> pomerančeKeSklizeni = new List<int>();
            for(int i = 0; i < početPomerančů; i++)
            {
                pomerančeKeSklizeni.Add(int.Parse(input[index]));
                index++;
            }
            return (values, int.Parse(input[0]), pomerančeKeSklizeni);
        }
    }
    class Vrchol : IVrchol
    {
        public int absolutniHodnota { get; set; }
        public int pocetPomernacu { get; set; }
        public IVrchol potomek { get; set; }
        public List<IVrchol> potomci { get; set; }
        public List<IVrchol> rodiče { get; set; }
        public List<IVrchol> sousedi { get; set; }
        public int index { get; set; }
        public List<IVrchol> sousedicirodiče { get; set; }
        public void SmazaniVrcholu()
        {
            throw new NotImplementedException();
        }
    }
    class Stromek : IStrom
    {
        public List<IVrchol> vrcholyGrafu { get; set ; }
        public int SklidPomerance(int PočetPomerancuKolikChceKamion)
        {
            AktualizovatAbsolutniHodnotyVrcholů();
            //Console.WriteLine("Chce tolik pomerančů: " + PočetPomerancuKolikChceKamion.ToString());
            bool pujdeToSnadno = false;
            foreach (IVrchol vrchol in vrcholyGrafu)
            {
                if (vrchol.absolutniHodnota == PočetPomerancuKolikChceKamion)
                {
                    //Console.WriteLine("Sklidit: " + vrchol.index.ToString());
                    pujdeToSnadno = true;
                }
            }
            if(pujdeToSnadno == false)
            {
                List<List<IVrchol>> kombinace = KombinaceVšechSečtitelnýchVrcholů();
                Console.WriteLine(kombinace.Count);
                List<IVrchol> vrcholyKeSklizeni = vyberVhodneVrcholyKeSklizeni(PočetPomerancuKolikChceKamion, kombinace);
                Console.WriteLine("Vrcholy vhodné ke sklizení:");
                foreach(IVrchol vrcgol in  vrcholyKeSklizeni)
                {
                    Console.WriteLine(vrcgol.index.ToString());
                }
            }
            return 0;
        }
        public List<List<IVrchol>> KombinaceVšechSečtitelnýchVrcholů()
        {
            List<List<IVrchol>> všechnyKombinace = new List<List<IVrchol>>();
            foreach (IVrchol vrchol in vrcholyGrafu)
            {
                if (vrchol != start)
                {
                    List<IVrchol> vrcholyDoKombinace = new List<IVrchol>();
                    foreach (IVrchol vrcholKeKombinaci in vrcholyGrafu)
                    {
                        if (vrcholKeKombinaci == start)
                        {

                        }
                        else if (vrcholKeKombinaci == vrchol)
                        {
                            vrcholyDoKombinace.Add(vrcholKeKombinaci);
                        }
                        else if (vrchol.rodiče.Contains(vrcholKeKombinaci) == false && vrcholKeKombinaci.rodiče.Contains(vrchol) == false)
                        {
                            vrcholyDoKombinace.Add(vrcholKeKombinaci);
                        }
                    }
                    //Console.WriteLine("Vrcholy do kombinace jsou:");
                    //foreach(IVrchol vrcholDoKombinace in vrcholyDoKombinace)
                    //{
                    //    Console.WriteLine("-"+vrcholDoKombinace.index.ToString());
                    //}
                    List<List<IVrchol>> kombinaceZVrcholu = VytvoreniVsechKombinaci(vrcholyDoKombinace);
                    všechnyKombinace.AddRange(kombinaceZVrcholu);
                    //Console.WriteLine();
                    //Console.WriteLine("Možné kombinace jsou takovéto:");
                    //foreach(var item in  kombinaceZVrcholu)
                    //{
                    //    foreach(IVrchol vrcholVKombinaci in item)
                    //    {
                    //        Console.Write(vrcholVKombinaci.index.ToString()+" ");
                    //    }
                    //    Console.WriteLine();
                    //}
                }
            }
            Console.ReadLine();
            return všechnyKombinace;
        }
        public List<IVrchol> vyberVhodneVrcholyKeSklizeni(int PomerancuKolikChceKamion, List<List<IVrchol>> všechnyKombinace)
        {
            List<IVrchol> vhodneVrcholy = new List<IVrchol>();
            foreach(List<IVrchol> kombinaceVrcholu in všechnyKombinace)
            {
                if(SoučetAbsolutnichHodnotVrcholu(kombinaceVrcholu) == PomerancuKolikChceKamion)
                {
                    //Console.WriteLine("Chce tolik pomerančů: "+PomerancuKolikChceKamion.ToString());
                    //Console.WriteLine("A tahle kombinace mu vyhovuje:");
                    //foreach(IVrchol vrchol in  kombinaceVrcholu)
                    //{
                    //    Console.WriteLine(vrchol.index.ToString());
                    //}
                    //Console.ReadLine();
                    return kombinaceVrcholu;
                }
                else if(vhodneVrcholy.Count == 0 && SoučetAbsolutnichHodnotVrcholu(kombinaceVrcholu) >= PomerancuKolikChceKamion)
                {
                    vhodneVrcholy = kombinaceVrcholu;
                }
                //else if(vhodneVrcholy.Count > 0 && SoučetAbsolutnichHodnotVrcholu(kombinaceVrcholu) >= PomerancuKolikChceKamion && SoučetAbsolutnichHodnotVrcholu(kombinaceVrcholu) < SoučetAbsolutnichHodnotVrcholu(vhodneVrcholy))
                //{
                //    vhodneVrcholy = kombinaceVrcholu;
                //}
            }
            return vhodneVrcholy;
        }
        public int SoučetAbsolutnichHodnotVrcholu(List<IVrchol> vrcholyKSečteni)
        {
            int součet = 0;
            foreach(IVrchol vrchol in vrcholyKSečteni)
            {
                součet += vrchol.absolutniHodnota;
            }
            return součet;
        }
        static List<List<IVrchol>> VytvoreniVsechKombinaci(List<IVrchol> vrcholy)
        {
            List<List<IVrchol>> vsechnyKombinace = new List<List<IVrchol>>();
            for (int i = 1; i <= vrcholy.Count; i++)
            {
                var kombinaceDaneDelky = VytvoreniKombinaciVrcholu(vrcholy, i);
                vsechnyKombinace.AddRange(kombinaceDaneDelky);
            }
            return vsechnyKombinace;
        }
        static List<List<IVrchol>> VytvoreniKombinaciVrcholu(List<IVrchol> vrcholy, int delka)
        {
            List<List<IVrchol>> kombinace = new List<List<IVrchol>>();
            Kombinace(vrcholy, 0, delka, new List<IVrchol>(), kombinace);
            List<List<IVrchol>> listyKOdebrani = new List<List<IVrchol>>();
            foreach(List<IVrchol> vrcholyVKombinaci in kombinace)
            {
                bool jeVPohodě = true;
                foreach(IVrchol vrchol1 in vrcholyVKombinaci)
                {
                    foreach (IVrchol vrchol2 in vrcholyVKombinaci)
                    {
                        if (vrchol1.rodiče.Contains(vrchol2) == true || vrchol2.rodiče.Contains(vrchol1) == true)
                        {
                            jeVPohodě = false;
                        }
                    }
                }
                if(jeVPohodě == false)
                {
                    listyKOdebrani.Add(vrcholyVKombinaci);
                }
            }
            foreach(var item in  listyKOdebrani)
            {
                kombinace.Remove(item);
            }
            return kombinace;
        }
        static void Kombinace(List<IVrchol> vrcholy, int index, int delka, List<IVrchol> aktualniKombinace, List<List<IVrchol>> vsechnyKombinace)
        {
            if (delka == 0)
            {
                vsechnyKombinace.Add(new List<IVrchol>(aktualniKombinace));
                return;
            }
            for (int i = index; i < vrcholy.Count; i++)
            {
                aktualniKombinace.Add(vrcholy[i]);
                Kombinace(vrcholy, i + 1, delka - 1, aktualniKombinace, vsechnyKombinace);
                aktualniKombinace.RemoveAt(aktualniKombinace.Count - 1);
            }
        }
        public IVrchol start { get; set; }
        public Dictionary<IVrchol, int> depths { get; set; }
        public void PriradVrcholumRodice()
        {
            foreach(IVrchol sousedStartu in start.sousedi)
            {
                RodiceVrcholu(sousedStartu);
            }
            //Console.WriteLine("Rodiče vrcholu:");
            //foreach (IVrchol vrchol in vrcholyGrafu)
            //{
            //    Console.WriteLine(vrchol.index.ToString() + ":");
            //    foreach (IVrchol rodic in vrchol.rodiče)
            //    {
            //        Console.WriteLine("-" + rodic.index.ToString());
            //    }
            //    Console.WriteLine();
            //}
        }
        public List<IVrchol> RodiceVrcholu(IVrchol vrchol)
        {
            List<IVrchol> rodice = new List<IVrchol>();
            foreach (IVrchol soused in vrchol.sousedi)
            {
                if (depths[soused] > depths[vrchol])
                {
                    if (soused.sousedi.Count == 1)
                    {
                        rodice.Add(soused);
                    }
                    else
                    {
                        List<IVrchol> rodiceSouseda = RodiceVrcholu(soused);
                        rodice.AddRange(rodiceSouseda);
                        rodice.Add(soused);
                    }
                }
            }
            vrchol.rodiče = rodice;
            return rodice;
        }
        public void AktualizovatAbsolutniHodnotyVrcholů()
        {
            foreach (IVrchol sousedStartu in start.sousedi)
            {
                AbsolutniHodnotaRodice(sousedStartu);
            }
            //Console.WriteLine("Absolutní hodnoty vrcholů:");
            //foreach (IVrchol vrchol in vrcholyGrafu)
            //{
            //    Console.WriteLine(vrchol.index.ToString() + ": " + vrchol.absolutniHodnota);
            //}
        }
        public int AbsolutniHodnotaRodice(IVrchol vrchol)
        {
            if(vrchol.rodiče.Count == 0)
            {
                vrchol.absolutniHodnota = vrchol.pocetPomernacu;
                return vrchol.absolutniHodnota;
            }
            else
            {
                //Console.WriteLine(vrchol.index.ToString()+ ":");
                int hodnotaRodiče = 0;
                foreach(IVrchol rodič in vrchol.sousedicirodiče)
                {
                    //Console.WriteLine(rodič.index);
                    hodnotaRodiče += AbsolutniHodnotaRodice(rodič);
                }
                vrchol.absolutniHodnota = hodnotaRodiče + vrchol.pocetPomernacu;
                return vrchol.absolutniHodnota;
            }
        }
        public void SestavGraf(List<(int Vrchol1, int Vrchol2, int PocetPomerancu)> values, int PocetVrcholu)
        {
            vrcholyGrafu = new List<IVrchol>();
            for (int i = 0; i < PocetVrcholu; i++)
            {
                Vrchol vrchol = new Vrchol();
                vrchol.index = i;
                vrchol.sousedi = new List<IVrchol>();
                vrchol.rodiče = new List<IVrchol>();
                vrchol.sousedicirodiče = new List<IVrchol>();
                vrcholyGrafu.Add(vrchol);
            }
            start = vrcholyGrafu[0];
            foreach(var item in  values)
            {
                vrcholyGrafu[item.Vrchol1].sousedi.Add(vrcholyGrafu[item.Vrchol2]);
                vrcholyGrafu[item.Vrchol2].sousedi.Add(vrcholyGrafu[item.Vrchol1]);
            }
            depths = new Dictionary<IVrchol, int>();
            Queue<IVrchol> queue = new Queue<IVrchol>();
            queue.Enqueue(start);
            depths.Add(start, 0);
            while (queue.Count > 0)
            {
                IVrchol vrchol = queue.Dequeue();
                List<IVrchol> sousedi = vrchol.sousedi;
                foreach(IVrchol soused in sousedi)
                {
                    if(depths.ContainsKey(soused) == false)
                    {
                        depths.Add(soused, depths[vrchol]+1);
                        queue.Enqueue(soused);
                    }
                }
            }
            //Console.WriteLine("Depths");
            //foreach(var item in depths)
            //{
            //    Console.WriteLine(item.Key.index.ToString()+": "+item.Value.ToString());
            //}
            foreach (var item in values)
            {
                IVrchol vrchol1 = vrcholyGrafu[item.Vrchol1];
                IVrchol vrchol2 = vrcholyGrafu[item.Vrchol2];
                if (depths[vrchol1] > depths[vrchol2])
                {
                    vrchol1.pocetPomernacu = item.PocetPomerancu;
                    vrchol2.sousedicirodiče.Add(vrchol1);
                }
                else
                {
                    vrchol2.pocetPomernacu = item.PocetPomerancu;
                    vrchol1.sousedicirodiče.Add(vrchol2);
                }
            }
            //Console.WriteLine("Pocet pomerančů");
            //foreach (var item in vrcholyGrafu)
            //{
            //    Console.WriteLine(item.index.ToString() + ": " + item.pocetPomernacu.ToString());
            //}
            PriradVrcholumRodice();
        }
        public void SklidVrcholy(List<IVrchol> vrcholyKeSklizeni)
        {
            throw new NotImplementedException();
        }
    }
    interface IVrchol
    {
        int index { get; set; }
        List<IVrchol> sousedi { get; set; }
        int absolutniHodnota { get; set; }
        int pocetPomernacu { get; set; }
        //int naKolikaZavisiAliasKolikMaNesklizenychPotomku { get; set; }
        IVrchol potomek { get; set; }
        List<IVrchol> potomci { get; set; }
        List<IVrchol> rodiče { get; set; }
        List<IVrchol> sousedicirodiče { get; set; }
        void SmazaniVrcholu();
    }
    interface IStrom
    {
        List <IVrchol> vrcholyGrafu { get; set; }
        int SklidPomerance(int PomerancuKolikChceKamion);
        List<IVrchol> vyberVhodneVrcholyKeSklizeni(int PomerancuKolikChceKamion, List<List<IVrchol>> všechnyKombinace);
        void SklidVrcholy(List<IVrchol> vrcholyKeSklizeni);
        IVrchol start { get; set; }
        Dictionary<IVrchol, int> depths { get; set; }
        void SestavGraf(List<(int Vrchol1, int Vrchol2, int PocetPomerancu)> values, int PocetVrcholu);
        void PriradVrcholumRodice();
        void AktualizovatAbsolutniHodnotyVrcholů();
        List<IVrchol> RodiceVrcholu(IVrchol vrchol);
        int AbsolutniHodnotaRodice(IVrchol vrchol);
    }
}