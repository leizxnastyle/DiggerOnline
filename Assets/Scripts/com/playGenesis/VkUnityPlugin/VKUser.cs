using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKUser
	{
		public static List<VKUser> Deserialize(object[] Users)
		{
			List<VKUser> list = new List<VKUser>();
			foreach (object user in Users)
			{
				list.Add(VKUser.Deserialize(user));
			}
			return list;
		}

		public static VKUser Deserialize(object User)
		{
			VKUser vkuser = new VKUser();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)User;
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkuser.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("first_name", out obj2))
			{
				vkuser.first_name = (string)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("last_name", out obj3))
			{
				vkuser.last_name = (string)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("deactivated", out obj4))
			{
				vkuser.deactivated = (string)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("hidden", out obj5))
			{
				vkuser.hidden = (int)((long)obj5);
			}
			object obj6;
			if (dictionary.TryGetValue("verified", out obj6))
			{
				vkuser.verified = (int)((long)obj6);
			}
			object obj7;
			if (dictionary.TryGetValue("blacklisted", out obj7))
			{
				vkuser.blacklisted = (int)((long)obj7);
			}
			object obj8;
			if (dictionary.TryGetValue("sex", out obj8))
			{
				vkuser.sex = (int)((long)obj8);
			}
			object obj9;
			if (dictionary.TryGetValue("bdate", out obj9))
			{
				vkuser.bdate = (string)obj9;
			}
			object obj10;
			if (dictionary.TryGetValue("city", out obj10))
			{
				vkuser.city = (string)obj10;
			}
			object obj11;
			if (dictionary.TryGetValue("country", out obj11))
			{
				vkuser.country = (string)obj11;
			}
			object obj12;
			if (dictionary.TryGetValue("home_town", out obj12))
			{
				vkuser.home_town = (string)obj12;
			}
			object obj13;
			if (dictionary.TryGetValue("home_phone", out obj13))
			{
				vkuser.home_phone = (string)obj13;
			}
			object obj14;
			if (dictionary.TryGetValue("photo_50", out obj14))
			{
				vkuser.photo_50 = (string)obj14;
			}
			object obj15;
			if (dictionary.TryGetValue("photo_100", out obj15))
			{
				vkuser.photo_100 = (string)obj15;
			}
			object obj16;
			if (dictionary.TryGetValue("photo_200", out obj16))
			{
				vkuser.photo_200 = (string)obj16;
			}
			object obj17;
			if (dictionary.TryGetValue("photo_200_orig", out obj17))
			{
				vkuser.photo_200_orig = (string)obj17;
			}
			object obj18;
			if (dictionary.TryGetValue("photo_400_orig", out obj18))
			{
				vkuser.photo_400_orig = (string)obj18;
			}
			object obj19;
			if (dictionary.TryGetValue("photo_max", out obj19))
			{
				vkuser.photo_max = (string)obj19;
			}
			object obj20;
			if (dictionary.TryGetValue("photo_max_orig", out obj20))
			{
				vkuser.photo_max_orig = (string)obj20;
			}
			object obj21;
			if (dictionary.TryGetValue("online", out obj21))
			{
				vkuser.online = int.Parse(obj21.ToString());
			}
			object obj22;
			if (dictionary.TryGetValue("lists", out obj22))
			{
				vkuser.lists = new List<long>();
				foreach (object obj23 in ((List<object>)obj22))
				{
					vkuser.lists.Add((long)obj23);
				}
			}
			object obj24;
			if (dictionary.TryGetValue("domain", out obj24))
			{
				vkuser.domain = (string)obj24;
			}
			object obj25;
			if (dictionary.TryGetValue("has_mobile", out obj25))
			{
				vkuser.has_mobile = (int)((long)obj25);
			}
			object obj26;
			if (dictionary.TryGetValue("mobile_phone", out obj26))
			{
				vkuser.mobile_phone = (string)obj26;
			}
			if (dictionary.TryGetValue("home_phone", out obj13))
			{
				vkuser.home_phone = (string)obj13;
			}
			object obj27;
			if (dictionary.TryGetValue("site", out obj27))
			{
				vkuser.site = (string)obj27;
			}
			object obj28;
			if (dictionary.TryGetValue("university", out obj28))
			{
				vkuser.university = (long)obj28;
			}
			object obj29;
			if (dictionary.TryGetValue("university_name", out obj29))
			{
				vkuser.university_name = (string)obj29;
			}
			object obj30;
			if (dictionary.TryGetValue("faculty", out obj30))
			{
				vkuser.faculty = (long)obj30;
			}
			object obj31;
			if (dictionary.TryGetValue("faculty_name", out obj31))
			{
				vkuser.faculty_name = (string)obj31;
			}
			object obj32;
			if (dictionary.TryGetValue("graduation", out obj32))
			{
				vkuser.graduation = (int)((long)obj32);
			}
			object obj33;
			if (dictionary.TryGetValue("universities", out obj33))
			{
				List<VKUniversity> list = new List<VKUniversity>();
				List<object> list2 = (List<object>)obj33;
				foreach (object university in list2)
				{
					list.Add(VKUniversity.Deserialize(university));
				}
				vkuser.universities = list;
			}
			object obj34;
			if (dictionary.TryGetValue("schools", out obj34))
			{
				List<VKSchool> list3 = new List<VKSchool>();
				List<object> list4 = (List<object>)obj34;
				foreach (object school in list4)
				{
					list3.Add(VKSchool.Deserialize(school));
				}
				vkuser.schools = list3;
			}
			object obj35;
			if (dictionary.TryGetValue("status", out obj35))
			{
				vkuser.status = (string)obj35;
			}
			object audio;
			if (dictionary.TryGetValue("status_audio", out audio))
			{
				vkuser.status_audio = VKAudio.Deserialize(audio);
			}
			object obj36;
			if (dictionary.TryGetValue("followers_count", out obj36))
			{
				vkuser.followers_count = (int)((long)obj36);
			}
			object obj37;
			if (dictionary.TryGetValue("common_count", out obj37))
			{
				vkuser.common_count = (int)((long)obj37);
			}
			object countries;
			if (dictionary.TryGetValue("counters", out countries))
			{
				vkuser.counters = VKCounters.Deserialize(countries);
			}
			object userOccupation;
			if (dictionary.TryGetValue("occupation", out userOccupation))
			{
				vkuser.occupation = VKUserOccupation.Deserialize(userOccupation);
			}
			object obj38;
			if (dictionary.TryGetValue("nickname", out obj38))
			{
				vkuser.nickname = (string)obj38;
			}
			object obj39;
			if (dictionary.TryGetValue("relatives", out obj39))
			{
				List<object> list5 = (List<object>)obj39;
				List<VKUserRelative> list6 = new List<VKUserRelative>();
				foreach (object userRelative in list5)
				{
					list6.Add(VKUserRelative.Deserialize(userRelative));
				}
				vkuser.relatives = list6;
			}
			object obj40;
			if (dictionary.TryGetValue("relation", out obj40))
			{
				vkuser.relation = (int)((long)obj40);
			}
			object userPersonal;
			if (dictionary.TryGetValue("personal", out userPersonal))
			{
				vkuser.personal = VKUserPersonal.Deserialize(userPersonal);
			}
			object obj41;
			if (dictionary.TryGetValue("facebook", out obj41))
			{
				vkuser.facebook = (string)obj41;
			}
			object obj42;
			if (dictionary.TryGetValue("twitter", out obj42))
			{
				vkuser.twitter = (string)obj42;
			}
			object obj43;
			if (dictionary.TryGetValue("livejournal", out obj43))
			{
				vkuser.livejournal = (string)obj43;
			}
			object obj44;
			if (dictionary.TryGetValue("instagram", out obj44))
			{
				vkuser.instagram = (string)obj44;
			}
			object userExports;
			if (dictionary.TryGetValue("exports", out userExports))
			{
				vkuser.exports = VKUserExports.Deserialize(userExports);
			}
			object obj45;
			if (dictionary.TryGetValue("wall_comments", out obj45))
			{
				vkuser.wall_comments = (int)((long)obj45);
			}
			object obj46;
			if (dictionary.TryGetValue("activities", out obj46))
			{
				vkuser.activities = (string)obj46;
			}
			object obj47;
			if (dictionary.TryGetValue("interests", out obj47))
			{
				vkuser.interests = (string)obj47;
			}
			object obj48;
			if (dictionary.TryGetValue("movies", out obj48))
			{
				vkuser.movies = (string)obj48;
			}
			object obj49;
			if (dictionary.TryGetValue("tv", out obj49))
			{
				vkuser.tv = (string)obj49;
			}
			object obj50;
			if (dictionary.TryGetValue("books", out obj50))
			{
				vkuser.books = (string)obj50;
			}
			object obj51;
			if (dictionary.TryGetValue("games", out obj51))
			{
				vkuser.games = (string)obj51;
			}
			object obj52;
			if (dictionary.TryGetValue("about", out obj52))
			{
				vkuser.about = (string)obj52;
			}
			object obj53;
			if (dictionary.TryGetValue("quotes", out obj53))
			{
				vkuser.quotes = (string)obj53;
			}
			object obj54;
			if (dictionary.TryGetValue("can_post", out obj54))
			{
				vkuser.can_post = (int)((long)obj54);
			}
			object obj55;
			if (dictionary.TryGetValue("can_see_all_posts", out obj55))
			{
				vkuser.can_see_all_posts = (int)((long)obj55);
			}
			object obj56;
			if (dictionary.TryGetValue("can_see_audio", out obj56))
			{
				vkuser.can_see_audio = (int)((long)obj56);
			}
			object obj57;
			if (dictionary.TryGetValue("can_write_private_message", out obj57))
			{
				vkuser.can_write_private_message = (int)((long)obj57);
			}
			object obj58;
			if (dictionary.TryGetValue("timezone", out obj58))
			{
				vkuser.timezone = (int)((long)obj58);
			}
			object obj59;
			if (dictionary.TryGetValue("screen_name", out obj59))
			{
				vkuser.screen_name = (string)obj59;
			}
			object obj60;
			if (dictionary.TryGetValue("maiden_name", out obj60))
			{
				vkuser.maiden_name = (string)obj60;
			}
			return vkuser;
		}

		public long id { get; set; }

		public string first_name { get; set; }

		public string last_name { get; set; }

		public string deactivated { get; set; }

		public int hidden { get; set; }

		public int verified { get; set; }

		public int blacklisted { get; set; }

		public int sex { get; set; }

		public string bdate { get; set; }

		public string city { get; set; }

		public string country { get; set; }

		public string home_town { get; set; }

		public string photo_50 { get; set; }

		public string photo_100 { get; set; }

		public string photo_200_orig { get; set; }

		public string photo_200 { get; set; }

		public string photo_400_orig { get; set; }

		public string photo_max { get; set; }

		public string photo_max_orig { get; set; }

		public int online { get; set; }

		public List<long> lists { get; set; }

		public string domain { get; set; }

		public int has_mobile { get; set; }

		public string mobile_phone { get; set; }

		public string home_phone { get; set; }

		public string site { get; set; }

		public long university { get; set; }

		public string university_name { get; set; }

		public long faculty { get; set; }

		public string faculty_name { get; set; }

		public int graduation { get; set; }

		public List<VKUniversity> universities { get; set; }

		public List<VKSchool> schools { get; set; }

		public string status { get; set; }

		public VKAudio status_audio { get; set; }

		public int followers_count { get; set; }

		public int common_count { get; set; }

		public VKCounters counters { get; set; }

		public VKUserOccupation occupation { get; set; }

		public string nickname { get; set; }

		public List<VKUserRelative> relatives { get; set; }

		public int relation { get; set; }

		public VKUserPersonal personal { get; set; }

		public string facebook { get; set; }

		public string twitter { get; set; }

		public string livejournal { get; set; }

		public string instagram { get; set; }

		public VKUserExports exports { get; set; }

		public int wall_comments { get; set; }

		public string activities { get; set; }

		public string interests { get; set; }

		public string movies { get; set; }

		public string tv { get; set; }

		public string books { get; set; }

		public string games { get; set; }

		public string about { get; set; }

		public string quotes { get; set; }

		public int can_post { get; set; }

		public int can_see_all_posts { get; set; }

		public int can_see_audio { get; set; }

		public int can_write_private_message { get; set; }

		public int timezone { get; set; }

		public string screen_name { get; set; }

		public string maiden_name { get; set; }
	}
}
