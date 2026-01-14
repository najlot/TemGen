using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Todo.ClientBase.Models;

namespace Todo.ClientBase.Services.Implementation;

public class ProfilesService : IProfilesService
{
	public class KnownTypesBinder : ISerializationBinder
	{
		public IList<Type> KnownTypes { get; set; }

		public Type BindToType(string assemblyName, string typeName)
		{
			return KnownTypes.SingleOrDefault(t => t.Name == typeName);
		}

		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = null;
			typeName = serializedType.Name;
		}
	}

	private static readonly string _profilesPath = BuildProfilesPath();
	private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
	{
		TypeNameHandling = TypeNameHandling.Objects,
		SerializationBinder = new KnownTypesBinder()
		{
			KnownTypes = new List<Type>
			{
				typeof(LocalProfile),
				typeof(RestProfile),
				typeof(RmqProfile)
			}
		}
	};

	private List<ProfileBase> _profiles;

	private static string BuildProfilesPath()
	{
		var appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Todo");
		Directory.CreateDirectory(appdataDir);
		return Path.Combine(appdataDir, "Profiles.json");
	}

	public async Task<List<ProfileBase>> LoadAsync()
	{
		// Take cache if not null
		if (_profiles != null)
		{
			return _profiles;
		}

		// Try load from file
		if (File.Exists(_profilesPath))
		{
#if NETSTANDARD2_1
			var content = await File.ReadAllTextAsync(_profilesPath);
#else
			var content = File.ReadAllText(_profilesPath);
#endif
			_profiles = JsonConvert.DeserializeObject<List<ProfileBase>>(content, _jsonSerializerSettings);

			return _profiles;
		}

		var id = Guid.NewGuid();

		var list = new List<ProfileBase>()
		{
			new LocalProfile()
			{
				Id = id,
				Name = "Local",
				FolderName = id.ToString(),
				Source = Source.Local
			}
		};

		await SaveAsync(list);

		return list;
	}

	public async Task RemoveAsync(ProfileBase profile)
	{
		var profiles = _profiles.Where(p => p.Id != profile.Id).ToList();
		await SaveAsync(profiles);
	}

	public async Task SaveAsync(List<ProfileBase> profiles)
	{
		// Set cache
		_profiles = profiles;

		// Write to file
		var content = JsonConvert.SerializeObject(profiles, _jsonSerializerSettings);
#if NETSTANDARD2_1
		await File.WriteAllTextAsync(_profilesPath, content);
#else
		File.WriteAllText(_profilesPath, content);
		await Task.CompletedTask;
#endif
	}
}