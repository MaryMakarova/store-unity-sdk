public class FriendModel
{
	/// <summary>
	/// Friend id.
	/// </summary>
	public string Id;
	/// <summary>
	/// Friend nickname.
	/// </summary>
	public string Nickname;
	/// <summary>
	/// Friend avatar url.
	/// </summary>
	public string AvatarUrl;
	/// <summary>
	/// Friend online status.
	/// </summary>
	public UserOnlineStatus Status;
	/// <summary>
	/// Friend relationship.
	/// </summary>
	public UserRelationship Relationship;

	public override bool Equals(object obj)
	{
		return string.IsNullOrEmpty(Id) ? base.Equals(obj) : Id.Equals(obj);
	}

	public override int GetHashCode()
	{
		return string.IsNullOrEmpty(Id) ? base.GetHashCode() : Id.GetHashCode();
	}
}