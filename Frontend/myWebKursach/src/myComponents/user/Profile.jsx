import { useState, useEffect } from "react";
import { Grid } from "@chakra-ui/react";
import { profile } from "../../services/profile";
import UserCard from "./UserCard";
import UpdateProfile from "./UpdateProfile";

export default function Profile() {
  const [user, setUser] = useState(null);
  const [isEditing, setIsEditing] = useState(false);

  async function fetchUserProfile() {
    const result = await profile();
    if (result.success) {
      setUser(result.data);
    }
  }

  useEffect(() => {
    fetchUserProfile();
  }, []);

  return (
    <Grid
      justifyContent="center"
      justifyItems="center"
      mt={8}
      templateColumns={{
        base: "repeat(auto-fit, minmax(280px, 1fr))",
        sm: "repeat(auto-fit, minmax(300px, 1fr))",
        md: "repeat(auto-fit, minmax(350px, 1fr))",
        lg: "repeat(auto-fit, minmax(400px, 1fr))",
        xl: "repeat(auto-fit, minmax(450px, 1fr))",
        "2xl": "repeat(auto-fit, minmax(500px, 1fr))",
      }}
      gridAutoRows="1fr"
      gap={6}
      rowGap={8}
      alignItems="stretch"
      w="100%"
    >
      {user && !isEditing && (
        <UserCard user={user} onEdit={() => setIsEditing(true)} />
      )}

      {isEditing && (
        <UpdateProfile
          user={user}
          onUpdate={() => {
            setIsEditing(false);
            fetchUserProfile();
          }}
          // Новый проп onCancel
          onCancel={() => {
            setIsEditing(false);
          }}
        />
      )}
    </Grid>
  );
}