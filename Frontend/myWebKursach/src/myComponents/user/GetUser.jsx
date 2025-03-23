import { useState, useEffect } from "react";
import { Grid } from "@chakra-ui/react";
import { getUser} from "../../services/getUser";
import UserCard from "./UserCard";
import { useParams } from "react-router-dom";
import UpdateProfile from "./UpdateProfile";

export default function GetUser() {
  const [user, setUser] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const { id } = useParams();

  
  async function fetchGetUser() {
    const result = await getUser(id);
   
    if (result.success) {
      setUser(result.data);
    }


  }

  useEffect(() => {
    fetchGetUser();
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
        {    console.log(user)}
       {user && !isEditing && (
         <UserCard key={user.id} user={user}onEdit={() => setIsEditing(true)} />
       )}
if()
      {isEditing && (
        <UpdateProfile
          user={user}
          onUpdate={() => {
            setIsEditing(false);
            fetchGetUser();
          }}

          onCancel={() => {
            setIsEditing(false);
          }}
        />
      )}
    </Grid>
  );
}