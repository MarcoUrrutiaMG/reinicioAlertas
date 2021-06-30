using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AlertReset.Entities.AlertManagement.Response.WC1
{
    [Serializable]
    [DataContract]
    public class ResponseWc1
    {
        [DataMember]
        public string resultId { get; set; }
        [DataMember]
        public string referenceId { get; set; }
        [DataMember]
        public string matchStrength { get; set; }
        [DataMember]
        public string matchedTerm { get; set; }
        [DataMember]
        public string submittedTerm { get; set; }
        [DataMember]
        public string matchedNameType { get; set; }
        [DataMember]
        public List<SecondaryFieldResult> secondaryFieldResults { get; set; }
        [DataMember]
        public List<string> sources { get; set; }
        [DataMember]
        public List<string> categories { get; set; }
        [DataMember]
        public string creationDate { get; set; }
        [DataMember]
        public string modificationDate { get; set; }
        [DataMember]
        public ResolutionResponse resolution { get; set; }
        [DataMember]
        public ResultReview resultReview { get; set; }
        [DataMember]
        public string primaryName { get; set; }
        [DataMember]
        public List<Events> events { get; set; }
        [DataMember]
        public List<CountryLinks> countryLinks {get; set;}
        [DataMember]
        public List<IdentityDocuments> identityDocuments { get; set; }
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public string providerType { get; set; }
        [DataMember]
        public string gender { get; set; }
    }
    public class ResolutionResponse
    {
        [DataMember]
        public string reasonId { get; set; }
        [DataMember]
        public string resolutionDate { get; set; }
        [DataMember]
        public string resolutionRemark { get; set; }
        [DataMember]
        public string riskId { get; set; }
        [DataMember]
        public string statusId { get; set; }
    }
    public class SecondaryFieldResult
    {
        [DataMember]
        public Field field { get; set; }
        [DataMember]
        public List<string> fieldResult { get; set; }
        [DataMember]
        public string matchedDateTimeValue { get; set; }
        [DataMember]
        public string matchedValue { get; set; }
        [DataMember]
        public string submittedDateTimeValue { get; set; }
        [DataMember]
        public string submittedValue { get; set; }
        [DataMember]
        public string typeId { get; set; }
    }

    public class Field
    {
        [DataMember]
        public string dateTimeValue { get; set; }
        [DataMember]
        public string typeId { get; set; }
        [DataMember]
        public string value { get; set; }
    }

    public class IdentityDocuments
    {
        [DataMember]
        public Entity entity { get; set; }
        [DataMember]
        public string expiryDate { get; set; }
        [DataMember]
        public string issueDate { get; set; }
        [DataMember]
        public string issuer { get; set; }
        [DataMember]
        public LocationType locationType { get; set; }
        [DataMember]
        public string number { get; set; }
        [DataMember]
        public string type { get; set; }
    }

    public class Entity
    {
        [DataMember]
        public ActionEntity actions { get; set; }
        [DataMember]
        public bool active { get; set; }
        [DataMember]
        public List<Address> addresses { get; set; }
        [DataMember]
        public List<EntityAssociates> associates { get; set; }
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string comments { get; set; }
        [DataMember]
        public List<EntityContacts> contacts { get; set; }
        [DataMember]
        public List<CountryLinks> countryLinks { get; set; }
        [DataMember]
        public string creationDate { get; set; }
        [DataMember]
        public string deletionDate { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public List<EntityDetail> details { get; set; }
        [DataMember]
        public string entityId { get; set; }
        [DataMember]
        public List<string> entityType { get; set; }
        [DataMember]
        public string externalImportId { get; set; }
        [DataMember]
        public List<EntityFile> files { get; set; }
        [DataMember]
        public List<IdentityDocuments> identityDocuments { get; set; }
        [DataMember]
        public List<EntityImages> images { get; set; }
        [DataMember]
        public string lastAdjunctChangeDate { get; set; }
        [DataMember]
        public string modificationDate { get; set; }
        [DataMember]
        public List<EntityNames> names { get; set; }
        [DataMember]
        public List<CountryLinks> previousCountryLinks { get; set; }
        [DataMember]
        public Provider provider { get; set; }
        [DataMember]
        public List<string> sourceUris { get; set; }
        [DataMember]
        public List<ActionEntitySource> sources { get; set; }
        [DataMember]
        public string subCategory { get; set; }
        [DataMember]
        public List<string> updateCategory { get; set; }
        [DataMember]
        public EntityUpdatedDates updatedDates { get; set; }
        [DataMember]
        public List<EntityFile> weblinks { get; set; }
    }

    public class EntityUpdatedDates
    {
        [DataMember]
        public string ageUpdated { get; set; }
        [DataMember]
        public string aliasesUpdated { get; set; }
        [DataMember]
        public string alternativeSpellingUpdated { get; set; }
        [DataMember]
        public string asOfDateUpdated { get; set; }
        [DataMember]
        public string categoryUpdated { get; set; }
        [DataMember]
        public string citizenshipsUpdated { get; set; }
        [DataMember]
        public string companiesUpdated { get; set; }
        [DataMember]
        public string deceasedUpdated { get; set; }
        [DataMember]
        public string dobsUpdated { get; set; }
        [DataMember]
        public string eiUpdated { get; set; }
        [DataMember]
        public string enteredUpdated { get; set; }
        [DataMember]
        public string externalSourcesUpdated { get; set; }
        [DataMember]
        public string firstNameUpdated { get; set; }
        [DataMember]
        public string foreignAliasUpdated { get; set; }
        [DataMember]
        public string furtherInformationUpdated { get; set; }
        [DataMember]
        public string idNumbersUpdated { get; set; }
        [DataMember]
        public string keywordsUpdated { get; set; }
        [DataMember]
        public string lastNameUpdated { get; set; }
        [DataMember]
        public string linkedToUpdated { get; set; }
        [DataMember]
        public string locationsUpdated { get; set; }
        [DataMember]
        public string lowQualityAliasesUpdated { get; set; }
        [DataMember]
        public string passportsUpdated { get; set; }
        [DataMember]
        public string placeOfBirthUpdated { get; set; }
        [DataMember]
        public string positionUpdated { get; set; }
        [DataMember]
        public string ssnUpdated { get; set; }
        [DataMember]
        public string subCategoryUpdated { get; set; }
        [DataMember]
        public string titleUpdated { get; set; }
        [DataMember]
        public string updatecategoryUpdated { get; set; }
    }

    public class EntityNames
    {
        [DataMember]
        public string fullName { get; set; }
        [DataMember]
        public string givenName { get; set; }
        [DataMember]
        public LanguageCode languageCode { get; set; }
        [DataMember]
        public string lastName { get; set; }
        [DataMember]
        public string originalScript { get; set; }
        [DataMember]
        public string prefix { get; set; }
        [DataMember]
        public string suffix { get; set; }
        [DataMember]
        public List<string> type { get; set; }
    }

    public class LanguageCode
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string name { get; set; }
    }

    public class EntityImages
    {
        [DataMember]
        public string caption { get; set; }
        [DataMember]
        public int height { get; set; }
        [DataMember]
        public string imageUseCode { get; set; }
        [DataMember]
        public List<string> tags { get; set; }
        [DataMember]
        public string uri { get; set; }
        [DataMember]
        public int width { get; set; }
    }

    public class EntityFile
    {
        [DataMember]
        public string caption { get; set; }
        [DataMember]
        public List<string> tags { get; set; }
        [DataMember]
        public string uri { get; set; }
    }

    public class EntityDetail
    {
        [DataMember]
        public List<string> detailType { get; set; }
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string title { get; set; }
    }

    public class EntityContacts
    {
        [DataMember]
        public List<string> contactDetailType { get; set; }
        [DataMember]
        public Country country { get; set; }
        [DataMember]
        public string detail { get; set; }
    }

    public class EntityAssociates
    {
        [DataMember]
        public string associateEntityType { get; set; }
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public string creationDate { get; set; }
        [DataMember]
        public List<string> entityType { get; set; }
        [DataMember]
        public string modificationDate { get; set; }
        [DataMember]
        public bool reversed { get; set; }
        [DataMember]
        public List<string> targetCategories { get; set; }
        [DataMember]
        public string targetEntityId { get; set; }
        [DataMember]
        public string targetExternalImportId { get; set; }
        [DataMember]
        public string targetPrimaryName { get; set; }
        [DataMember]
        public List<string> type { get; set; }
        [DataMember]
        public List<string> updateCategory { get; set; }
    }

    public class ActionEntity
    {
        [DataMember]
        public string actionId { get; set; }
        [DataMember]
        public List<string> actionType { get; set; }
        [DataMember]
        public string comment { get; set; }
        [DataMember]
        public string endDate { get; set; }
        [DataMember]
        public List<File> files { get; set; }
        [DataMember]
        public string publicationType { get; set; }
        [DataMember]
        public string published { get; set; }
        [DataMember]
        public string reference { get; set; }
        [DataMember]
        public ActionEntitySource source { get; set; }
        [DataMember]
        public string startDate { get; set; }
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string title { get; set; }
    }

    public class ActionEntitySource
    {
        [DataMember]
        public string abbreviation { get; set; }
        [DataMember]
        public string creationDate { get; set; }
        [DataMember]
        public string identifier { get; set; }
        [DataMember]
        public string importIdentifier { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public Provider provider { get; set; }
        [DataMember]
        public List<string> providerSourceStatus { get; set; }
        [DataMember]
        public string regionOfAuthority { get; set; }
        [DataMember]
        public List<string> subscriptionCategory { get; set; }
        [DataMember]
        public TypeSource type { get; set; }
    }

    public class TypeSource
    {
        [DataMember]
        public TypeSourceCategory category { get; set; }
        [DataMember]
        public string identifier { get; set; }
        [DataMember]
        public string name { get; set; }
    }

    public class TypeSourceCategory
    {
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string identifier { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public TypeSource providerSourceTypes { get; set; }
    }


    public class Provider
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string identifier { get; set; }
        [DataMember]
        public bool master { get; set; }
        [DataMember]
        public string name { get; set; }
    }

    public class File
    {
        [DataMember]
        public string caption { get; set; }
        [DataMember]
        public string tags { get; set; }
        [DataMember]
        public string uri { get; set; }
    }

    public class LocationType
    {
        [DataMember]
        public Country country { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string type { get; set; }
    }

    public class CountryLinks
    {
        [DataMember]
        public Country country { get; set; }
        [DataMember]
        public string countryText { get; set; }
        [DataMember]
        public string type { get; set; }
    }

    public class Country
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string name { get; set; }
    }

    public class ResultReview
    {
        [DataMember]
        public bool reviewRequired { get; set; }
        [DataMember]
        public string reviewRequiredDate { get; set; }
        [DataMember]
        public string reviewRemark { get; set; }
        [DataMember]
        public string reviewDate { get; set; }
    }

    public class Events
    {
        [DataMember]
        public Address address { get; set; }
        [DataMember]
        public List<Address> allegedAddresses { get; set; }
        [DataMember]
        public string day { get; set; }
        [DataMember]
        public string fullDate { get; set; }
        [DataMember]
        public string month { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string year { get; set; }
    }

    public class Address
    {
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public Country country { get; set; }
        [DataMember]
        public string postCode { get; set; }
        [DataMember]
        public string region { get; set; }
        [DataMember]
        public string street { get; set; }
    }
}
