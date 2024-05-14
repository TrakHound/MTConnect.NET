// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class LocationAddressResult : EventDataSetObservation
    {

        /// <summary>
        /// Element identifying the number or name and type of the edifice or construction in or adjacent to which a delivery point is located.
        /// </summary>
        public string Building 
        { 
            get => GetValue<string>("DataSet[Building]");
            set => AddValue("DataSet[Building]", value);
        }
        
        /// <summary>
        /// Interest, in which a delivery point is located or via which the delivery point is accessed.
        /// </summary>
        public string CountryCode 
        { 
            get => GetValue<string>("DataSet[CountryCode]");
            set => AddValue("DataSet[CountryCode]", value);
        }
        
        /// <summary>
        /// Element designating the country, dependency or area of geopolitical interest, in which a delivery point is located or via which the delivery point is accessed.
        /// </summary>
        public string CountryName 
        { 
            get => GetValue<string>("DataSet[CountryName]");
            set => AddValue("DataSet[CountryName]", value);
        }
        
        /// <summary>
        /// Element indicating the name of the area within or adjacent to the town in which a delivery point is located, or via which it is accessed.
        /// </summary>
        public string District 
        { 
            get => GetValue<string>("DataSet[District]");
            set => AddValue("DataSet[District]", value);
        }
        
        /// <summary>
        /// Element indicating the apartment, room or office in, at or adjacent to which a delivery point, situated within a building, is located.
        /// </summary>
        public string Door 
        { 
            get => GetValue<string>("DataSet[Door]");
            set => AddValue("DataSet[Door]", value);
        }
        
        /// <summary>
        /// Element indicating the floor or level on which a delivery point is located in a multi-story building.
        /// </summary>
        public string Floor 
        { 
            get => GetValue<string>("DataSet[Floor]");
            set => AddValue("DataSet[Floor]", value);
        }
        
        /// <summary>
        /// Element specifying the name used to distinguish between persons having the same surname(s) and who may have access to a particular delivery point.
        /// </summary>
        public string GivenName 
        { 
            get => GetValue<string>("DataSet[GivenName]");
            set => AddValue("DataSet[GivenName]", value);
        }
        
        /// <summary>
        /// Element indicating the formal registration of an organization (e.g. GmbH, Inc., Ltd.).
        /// </summary>
        public string LegalStatus 
        { 
            get => GetValue<string>("DataSet[LegalStatus]");
            set => AddValue("DataSet[LegalStatus]", value);
        }
        
        /// <summary>
        /// Element used in some countries to distinguish between persons with the same surname(s) who have similar given names or initials (e.g.III, Senior, the Third.).
        /// </summary>
        public string NameQualifier 
        { 
            get => GetValue<string>("DataSet[NameQualifier]");
            set => AddValue("DataSet[NameQualifier]", value);
        }
        
        /// <summary>
        /// Element giving the official name, the registered business name or other official designation of an organization.
        /// </summary>
        public string OrganizationName 
        { 
            get => GetValue<string>("DataSet[OrganizationName]");
            set => AddValue("DataSet[OrganizationName]", value);
        }
        
        /// <summary>
        /// Element identifying a subdivision of an organization.
        /// </summary>
        public string OrganizationUnit 
        { 
            get => GetValue<string>("DataSet[OrganizationUnit]");
            set => AddValue("DataSet[OrganizationUnit]", value);
        }
        
        /// <summary>
        /// Element designating the code used for the sorting of mail.
        /// </summary>
        public string PostCode 
        { 
            get => GetValue<string>("DataSet[PostCode]");
            set => AddValue("DataSet[PostCode]", value);
        }
        
        /// <summary>
        /// Element designating the area or the object on an area, adjacent to thoroughfare, in which the delivery point or delivery point access is located.
        /// </summary>
        public string PremiseIdentifier 
        { 
            get => GetValue<string>("DataSet[PremiseIdentifier]");
            set => AddValue("DataSet[PremiseIdentifier]", value);
        }
        
        /// <summary>
        /// Element indicating an individual’s professional or academic qualification or rank in a professional group or society (e.g. PhD, Fellow of the Royal Society, FRS, Barrister at Law).
        /// </summary>
        public string Qualification 
        { 
            get => GetValue<string>("DataSet[Qualification]");
            set => AddValue("DataSet[Qualification]", value);
        }
        
        /// <summary>
        /// Element specifying the geographic or administrative area of the country in which the town is situated.
        /// </summary>
        public string Region 
        { 
            get => GetValue<string>("DataSet[Region]");
            set => AddValue("DataSet[Region]", value);
        }
        
        /// <summary>
        /// Element which identifies the family or parentage of an individual.
        /// </summary>
        public string Surname 
        { 
            get => GetValue<string>("DataSet[Surname]");
            set => AddValue("DataSet[Surname]", value);
        }
        
        /// <summary>
        /// Element which identifies the road or part of a road or other access route along which a delivery point can be accessed, either directly or via a secondary or tertiary road or access route.
        /// </summary>
        public string Thoroughfare 
        { 
            get => GetValue<string>("DataSet[Thoroughfare]");
            set => AddValue("DataSet[Thoroughfare]", value);
        }
        
        /// <summary>
        /// Element indicating the name of the populated place in which a delivery point is located, or the populated.
        /// </summary>
        public string Town 
        { 
            get => GetValue<string>("DataSet[Town]");
            set => AddValue("DataSet[Town]", value);
        }
    }
}