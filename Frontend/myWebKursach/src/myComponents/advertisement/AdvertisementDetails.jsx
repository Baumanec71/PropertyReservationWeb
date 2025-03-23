import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { Grid } from "@chakra-ui/react";
import axios from "axios";
import { getAdvertisement } from "../../services/advertisements/getAdvertisement";
import AdvertisementCardFull from "./AdvertisementCardFull";
import UpdateAdvertisement from "./UpdateAdvertisement";

export default function AdvertisementDetails() {
    const { id } = useParams();
    const [loading, setLoading] = useState(true);
    const [isEditing, setIsEditing] = useState(false);
    const [advertisement, setAdvertisement] = useState(null);
  async function fetchAdvertisement() {
    const result = await getAdvertisement(id);
    if (result.success) {
        setAdvertisement(result.data);
    }
  }

  useEffect(() => {
    fetchAdvertisement();
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
       {advertisement && !isEditing && (
         <AdvertisementCardFull key={advertisement.id} ad={advertisement} onEdit={() => setIsEditing(true)} />
       )}
 
       {isEditing && (
         <UpdateAdvertisement
         ad={advertisement}
         id = {advertisement.id}
           onUpdate={() => {
             setIsEditing(false);
             fetchAdvertisement();
           }}
           onCancel={() => {
             setIsEditing(false);
           }}
         />
       )}
     </Grid>
   );
 };