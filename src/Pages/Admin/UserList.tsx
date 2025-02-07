import { useEffect, useState } from "react";
import { User } from "../../Types/User";

const UserList = () => {
  const [userList, setUserList] = useState<User[]>([]);
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
      } catch (error) {
        console.error("Failed to fetch user data:", error);
        localStorage.removeItem("accessToken");
      }
    };

    fetchUserData();
  }, []);

  return (
    <div>
      <h2>User List</h2>
      <table border="1" cellPadding="5">
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
          {userList.map((user) => (
            <tr key={user.id}>
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
  );
};

export default UserList;
