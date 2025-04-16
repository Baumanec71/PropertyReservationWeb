import { useState, useEffect } from "react";
import { Grid } from "@chakra-ui/react";
import { getRentalRequest } from "@/services/rentalRequests/getRentalRequest";
import { useParams } from "react-router-dom";
import RentalRequestCard from "./RentalRequestCard";

export default function GetRentalRequest() {
  const [rentalRequest, setRentalRequest] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const { id } = useParams();

  
  async function fetchGetRentalRequest() {
    const result = await getRentalRequest(id);
   
    if (result.success) {
      setRentalRequest(result.data);
    }


  }

  useEffect(() => {
    fetchGetRentalRequest();
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
        {    console.log(rentalRequest)}
       {rentalRequest && !isEditing && (
         <RentalRequestCard key={rentalRequest.id} request={rentalRequest}onEdit={() => setIsEditing(true)} />
       )}
    </Grid>
  );
}