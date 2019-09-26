using System;
using Newtonsoft.Json;
using Starcounter.Nova;

namespace Starcounter.MultiModelBenchmark
{
    [Database]
    public abstract class ProfileBase
    {
        [JsonProperty("_key")]
        public abstract int Key { get; set; }

        [JsonProperty("public")]
        public abstract int? Public { get; set; }

        [JsonProperty("completion_percentage")]
        public abstract int? CompletionPercentage { get; set; }

        [JsonProperty("gender")]
        public abstract int? Gender { get; set; }

        [JsonProperty("region")]
        public abstract string Region { get; set; }

        [JsonProperty("last_login")]
        public abstract DateTime? LastLogin { get; set; }

        [JsonProperty("registration")]
        public abstract DateTime? Registration { get; set; }

        [JsonProperty("age")]
        public abstract int? Age { get; set; }

        [JsonProperty("body")]
        public abstract string Body { get; set; }

        [JsonProperty("i_am_working_in_field")]
        public abstract string WorkFieldDescription { get; set; }

        [JsonProperty("spoken_languages")]
        public abstract string SpokenLanguages { get; set; }

        [JsonProperty("hobbies")]
        public abstract string Hobbies { get; set; }

        [JsonProperty("i_most_enjoy_good_food")]
        public abstract string EnjoyFoodAt { get; set; }

        [JsonProperty("pets")]
        public abstract string Pets { get; set; }

        [JsonProperty("body_type")]
        public abstract string BodyType { get; set; }

        [JsonProperty("my_eyesight")]
        public abstract string EyeSight { get; set; }

        [JsonProperty("eye_color")]
        public abstract string EyeColor { get; set; }

        [JsonProperty("hair_color")]
        public abstract string HairColor { get; set; }

        [JsonProperty("hair_type")]
        public abstract string HairType { get; set; }

        [JsonProperty("completed_level_of_education")]
        public abstract string CompletedEducationLevel { get; set; }

        [JsonProperty("favourite_color")]
        public abstract string FavoriteColor { get; set; }

        [JsonProperty("relation_to_smoking")]
        public abstract string SmokingAttitude { get; set; }

        [JsonProperty("relation_to_alcohol")]
        public abstract string AlcoholAttitude { get; set; }

        [JsonProperty("sign_in_zodiac")]
        public abstract string ZodiacSign { get; set; }

        [JsonProperty("on_pokec_i_am_looking_for")]
        public abstract string LookingFor { get; set; }

        [JsonProperty("love_is_for_me")]
        public abstract string LoveAttitude { get; set; }

        [JsonProperty("relation_to_casual_sex")]
        public abstract string RandomSexAttitude { get; set; }

        [JsonProperty("my_partner_should_be")]
        public abstract string WantedPartnerDescription { get; set; }

        [JsonProperty("marital_status")]
        public abstract string MaritalStatus { get; set; }

        [JsonProperty("children")]
        public abstract string Children { get; set; }

        [JsonProperty("relation_to_children")]
        public abstract string ChildrenAttitude { get; set; }

        [JsonProperty("i_like_movies")]
        public abstract string LikeMovies { get; set; }

        [JsonProperty("i_like_watching_movie")]
        public abstract string EnjoyMoviesAt { get; set; }

        [JsonProperty("i_like_music")]
        public abstract string LikeMusic { get; set; }

        [JsonProperty("i_mostly_like_listening_to_music")]
        public abstract string EnjoyMusicAt { get; set; }

        [JsonProperty("the_idea_of_good_evening")]
        public abstract string GoodEveningDescription { get; set; }

        [JsonProperty("i_like_specialities_from_kitchen")]
        public abstract string LikeKitchenFrom { get; set; }

        [JsonProperty("fun")]
        public abstract string Fun { get; set; }

        [JsonProperty("i_am_going_to_concerts")]
        public abstract string ConcertsAttitude { get; set; }

        [JsonProperty("my_active_sports")]
        public abstract string ActiveSports { get; set; }

        [JsonProperty("my_passive_sports")]
        public abstract string PassiveSports { get; set; }

        [JsonProperty("profession")]
        public abstract string Profession { get; set; }

        [JsonProperty("i_like_books")]
        public abstract string LikeBooks { get; set; }

        [JsonProperty("life_style")]
        public abstract string LifeStyle { get; set; }

        [JsonProperty("music")]
        public abstract string Music { get; set; }

        [JsonProperty("cars")]
        public abstract string Cars { get; set; }

        [JsonProperty("politics")]
        public abstract string Politics { get; set; }

        [JsonProperty("relationships")]
        public abstract string Relationships { get; set; }

        [JsonProperty("art_culture")]
        public abstract string ArtCulture { get; set; }

        [JsonProperty("hobbies_interestes")]
        public abstract string Interests { get; set; }

        [JsonProperty("science_technologies")]
        public abstract string ScienceTechnologies { get; set; }

        [JsonProperty("computers_internet")]
        public abstract string ComputersInternet { get; set; }

        [JsonProperty("education")]
        public abstract string Education { get; set; }

        [JsonProperty("sports")]
        public abstract string Sports { get; set; }

        [JsonProperty("movies")]
        public abstract string Movies { get; set; }

        [JsonProperty("travelling")]
        public abstract string Travelling { get; set; }

        [JsonProperty("health")]
        public abstract string Health { get; set; }

        [JsonProperty("companies_brands")]
        public abstract string CompaniesBrands { get; set; }

        [JsonProperty("more")]
        public abstract string More { get; set; }
    }
}
