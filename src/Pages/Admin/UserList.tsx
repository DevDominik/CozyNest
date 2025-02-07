import { useEffect, useState } from "react";
import { User } from "../../Types/User";
import Styles from "./Admin.module.css";

const UserList = () => {
  const [userList, setUserList] = useState<User[]>([]);
  const [filteredUsers, setFilteredUsers] = useState<User[]>([]);
  const [userInfo, setUserInfo] = useState<string>("");
  const [searchQuery, setSearchQuery] = useState<string>("");
  const API_URL = "https://localhost:7290";

  useEffect(() => {
    const fetchUserData = async () => {
      const token = localStorage.getItem("accessToken");
      if (!token) {
        return;
      }

      try {
        const response = await fetch(`${API_URL}/api/admin/getusers`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ accessToken: token }),
        });

        const data = await response.json();
        if (!response.ok) {
          throw new Error(data.message);
        }
        setUserList(data.users);
        setFilteredUsers(data.users); // Initialize filtered list
      } catch (error) {
        console.error("Failed to fetch user data:", error);
        localStorage.removeItem("accessToken");
      }
    };

    fetchUserData();
  }, []);

  const loadUserInfo = (user: User) => {
    setUserInfo(
      `${user.username} - ${user.firstName ? user.firstName : "Empty"} ${
        user.lastName ? user.lastName : "Empty"
      } - ${user.email}`
    );
  };

  const handleSearch = (event: React.ChangeEvent<HTMLInputElement>) => {
    const query = event.target.value.toLowerCase();
    setSearchQuery(query);

    const filtered = userList.filter(
      (user) =>
        user.email.toLowerCase().includes(query) ||
        user.username.toLowerCase().includes(query)
    );

    setFilteredUsers(filtered);
  };

  return (
    <div>
      <h2>User List</h2>
      <div className={Styles.scroll}>
        <table className={Styles.userList} cellPadding="5">
          <thead>
            <tr>
              <th>ID</th>
              <th>Email</th>
              <th>Username</th>
              <th>First Name</th>
              <th>Last Name</th>
              <th>Closed</th>
              <th>Join Date</th>
              <th>Role Name</th>
            </tr>
          </thead>
          <tbody>
            {filteredUsers.map((user) => (
              <tr key={user.id} onClick={() => loadUserInfo(user)}>
                <td>{user.id}</td>
                <td>{user.email}</td>
                <td>{user.username}</td>
                <td>{user.firstName}</td>
                <td>{user.lastName}</td>
                <td>{user.closed ? "Yes" : "No"}</td>
                <td>{new Date(user.joinDate).toLocaleDateString()}</td>
                <td>{user.roleName}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className={Styles.menu}>
        <input
          type="text"
          placeholder="Search by Email or Username"
          value={searchQuery}
          onChange={handleSearch}
          className={Styles.searchBar}
        />

        <input type="text" value={userInfo} readOnly />
      </div>
    </div>
  );
};

export default UserList;
