using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace FarCry2_trainer
{
    class ParseGameFiles
    {


        public List<string> GetDirectChildren(XmlDocument doc)
        {
            List<string> results = new List<string> { };
            for (int i = 0; i < doc.ChildNodes[0].ChildNodes.Count; i++)
            {
                XmlNode node = doc.ChildNodes[0].ChildNodes[i];
                if (node.HasChildNodes)
                {
                    string name = node.ChildNodes[0].InnerText;
                    results.Add(name);
                }
            }
            return results;
        }


        public Dictionary<string, string> GetInfo(XmlDocument doc, string name, string file)
        {
            Dictionary<string, string> result = new Dictionary<string, string> { };

            switch (file)
            {
                case @"41_WeaponProperties.xml":

                    XmlNode weapon = doc.SelectSingleNode(".//value[text()='" + name + "']").NextSibling;
                    XmlNode MuzzleStims = weapon.SelectSingleNode(".//object[@type='MuzzleStims']");
                    XmlNode ImpactStims = weapon.SelectSingleNode(".//object[@type='ImpactStims']");

                    XmlNode CommonProperties = weapon.SelectSingleNode(".//object[@type='CommonProperties']");
                    XmlNode FireStrategyProperties = weapon.SelectSingleNode(".//object[@type='FireStrategyProperties']");
                    

                    try
                    {
                        result.Add("selCategory", CommonProperties.SelectSingleNode(".//value[@name='selCategory']").InnerText);
                        result.Add("sound", FireStrategyProperties.SelectSingleNode(".//value[@hash='876895B3']").InnerText);
                        result.Add("m l ", MuzzleStims.SelectSingleNode(".//value[@name='nLevel']").InnerText);
                        result.Add("m r ", MuzzleStims.SelectSingleNode(".//value[@name='fRadius']").InnerText);
                        result.Add("i l ", ImpactStims.SelectSingleNode(".//value[@name='nLevel']").InnerText);
                        result.Add("i r ", ImpactStims.SelectSingleNode(".//value[@name='fRadius']").InnerText);
                    }
                    catch (Exception exp)
                    {
                        result.Add("stim error", exp.ToString());
                    }                    
                    break;

                default:
                    break;                    
            }

            result.Add("END OF TRACKED PARAM", "");

            XmlNode node = doc.SelectSingleNode(".//value[text()='" + name + "']").NextSibling;
            result = GetDetail(node, result);

            return result;
        }


        public Dictionary<string, string> GetDetail(XmlNode node, Dictionary<string, string> result)
        {

            foreach (XmlNode c in node.ChildNodes)
            {

                if (c.Attributes != null)
                {
                    bool isEnum = false;

                    foreach (XmlAttribute a in c.Attributes)
                    {
                        if (a.Value == "enum")
                            isEnum = true;

                        if (a.Name == "name")
                        {
                            string value = c.Attributes[0].Value;

                            int i = 2;
                            while (result.Keys.Contains(value))
                            {
                                value = value.Split('_')[0] + '_' + i.ToString();
                                i++;
                            }

                            result.Add(value, c.InnerText);
                        }
                    }

                    if (!isEnum)
                        result = GetDetail(c, result);
                }
            }
            
            return result;

        }


        #region xml mod operations

        public void MakeSilent(ref XmlDocument doc, string name)
        {
            XmlNode weapon = doc.SelectSingleNode(".//value[text()='" + name + "']").NextSibling;
            XmlNode MuzzleStims = weapon.SelectSingleNode(".//object[@type='MuzzleStims']");
            XmlNode ImpactStims = weapon.SelectSingleNode(".//object[@type='ImpactStims']");

            MuzzleStims.SelectSingleNode(".//value[@name='nLevel']").InnerText = "0";
            MuzzleStims.SelectSingleNode(".//value[@name='fRadius']").InnerText = "0";

            ImpactStims.SelectSingleNode(".//value[@name='nLevel']").InnerText = "0";
            ImpactStims.SelectSingleNode(".//value[@name='fRadius']").InnerText = "0";

            XmlNode CommonProperties = weapon.SelectSingleNode(".//object[@type='CommonProperties']");
            CommonProperties.SelectSingleNode(".//value[@name='bIsSilent']").InnerText = "True";

        }

        public void MakePrimary(ref XmlDocument doc, string name)
        {
            XmlNode weapon = doc.SelectSingleNode(".//value[text()='" + name + "']").NextSibling;
            XmlNode CommonProperties = weapon.SelectSingleNode(".//object[@type='CommonProperties']");
            CommonProperties.SelectSingleNode(".//value[@name='selCategory']").InnerText = "1";
        }

        public void MakeSecondary(ref XmlDocument doc, string name)
        {
            XmlNode weapon = doc.SelectSingleNode(".//value[text()='" + name + "']").NextSibling;
            XmlNode CommonProperties = weapon.SelectSingleNode(".//object[@type='CommonProperties']");
            CommonProperties.SelectSingleNode(".//value[@name='selCategory']").InnerText = "2";
        }

        public void MakeSpecial(ref XmlDocument doc, string name)
        {
            XmlNode weapon = doc.SelectSingleNode(".//value[text()='" + name + "']").NextSibling;
            XmlNode CommonProperties = weapon.SelectSingleNode(".//object[@type='CommonProperties']");
            CommonProperties.SelectSingleNode(".//value[@name='selCategory']").InnerText = "3";
        }

        public void MakeDartRifleSound(ref XmlDocument doc, string name)
        {
            string ref_name = "Special.Dart_Rifle";

            XmlNode weapon = doc.SelectSingleNode(".//value[text()='" + ref_name + "']").NextSibling;
            XmlNode CommonProperties = weapon.SelectSingleNode(".//object[@type='FireStrategyProperties']");
            string ref_sound = CommonProperties.SelectSingleNode(".//value[@hash='876895B3']").InnerText;


            weapon = doc.SelectSingleNode(".//value[text()='" + name + "']").NextSibling;
            CommonProperties = weapon.SelectSingleNode(".//object[@type='FireStrategyProperties']");
            CommonProperties.SelectSingleNode(".//value[@hash='876895B3']").InnerText = ref_sound;

        }

        public void RemoveSpread(ref XmlDocument doc, string name)
        {
            XmlNode weapon = doc.SelectSingleNode(".//value[text()='" + name + "']").NextSibling;
            XmlNode CommonProperties = weapon.SelectSingleNode(".//object[@type='FireStrategyProperties']");

            CommonProperties.SelectSingleNode(".//value[@name='bUseAngleSpread']").InnerText = "True";
            CommonProperties.SelectSingleNode(".//value[@name='fAngleYawBulletSpread']").InnerText = "0";
            CommonProperties.SelectSingleNode(".//value[@name='fAnglePitchBulletSpread']").InnerText = "0";           
        }
       
        public void MakeClimber(ref XmlDocument doc, string name)
        {
            //MainCharacter.PawnPlayer.Paul_Ferenc
            XmlNode player = doc.SelectSingleNode(".//value[text()='" + name + "']").NextSibling;
            player.SelectSingleNode(".//value[@name='fMaxSlope']").InnerText = "90";
            player.SelectSingleNode(".//value[@name='fMaxTerrainSlope']").InnerText = "90";
        }

        public void MakeRespawn(ref XmlDocument doc, string name)
        {
            XmlNode player = doc.SelectSingleNode(".//value[text()='" + name + "']").NextSibling;
            player.SelectSingleNode(".//value[@name='fRespawnTime']").InnerText = "0.1";
        }

        public void UnlockWeapons(ref XmlDocument doc)
        {
            XmlNode bazaar = doc.SelectSingleNode(".//WeaponBazaar");
            XmlNodeList weapons = bazaar.SelectNodes(".//Item[@category='weapons']");
            foreach(XmlNode n in weapons)
            {
                n.Attributes["availability"].Value = "0";
            }
        }

        public void UnlockEquipment(ref XmlDocument doc)
        {
            XmlNode bazaar = doc.SelectSingleNode(".//WeaponBazaar");
            XmlNodeList equipment = bazaar.SelectNodes(".//Item[@category='equipment']");
            foreach (XmlNode n in equipment)
            {
                n.Attributes["availability"].Value = "0";
            }
        }

        public void SetManualCost(ref XmlDocument doc, string cost)
        {
            XmlNode bazaar = doc.SelectSingleNode(".//WeaponBazaar");
            XmlNodeList manual = bazaar.SelectNodes(".//Item[@category='manual']");
            foreach (XmlNode n in manual)
            {
                n.Attributes["cost"].Value = cost;
            }
        }

        public void RemoveMalaria(ref XmlDocument doc)
        {
            XmlNode DefaultCountersService = doc.SelectSingleNode(".//DefaultCountersService");
            XmlNode Malaria = DefaultCountersService.SelectSingleNode(".//Malaria");

            Malaria.Attributes["FirstAttackTime"].Value = "Curves.PlayerSicknessCurves.HealthMax_Infamous";
            Malaria.Attributes["BetweenAttackTime"].Value = "Curves.PlayerSicknessCurves.HealthMax_Infamous";
            Malaria.Attributes["MinorAttackQte"].Value = "Curves.PlayerSicknessCurves.HealthMax_Infamous";
            Malaria.Attributes["MinorAttackDuration"].Value = "Curves.PlayerSicknessCurves.HealthMax_Infamous";
        }

        public void UnlimitedSprint(ref XmlDocument doc)
        {
            XmlNode DefaultCountersService = doc.SelectSingleNode(".//DefaultCountersService");
            XmlNode Malaria = DefaultCountersService.SelectSingleNode(".//Stamina");
            //RegenRate = "Curves.PlayerSicknessCurves.StaminaMax"
            Malaria.Attributes["RegenRate"].Value = "Curves.PlayerSicknessCurves.StaminaMax";
        }

        #endregion
    }
}
