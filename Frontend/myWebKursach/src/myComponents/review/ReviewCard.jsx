import {
    Card,
    Stack,
    Text,
    Avatar,
    Box,
    RatingGroup, HStack, VStack, Button
  } from "@chakra-ui/react";
  import { getUserRole } from "../../services/InfoJwt/getUserRole";
  import { getUserEmail } from "../../services/InfoJwt/getUserEmail";
  import { useNavigate, useLocation } from "react-router-dom";
  import { useEffect, useState } from "react";
  import dayjs from "dayjs";
  
  export default function ReviewCard({ review }) {
    const [role, setRole] = useState(null);
    const [email, setEmail] = useState(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();
  
    useEffect(() => {
      const checkAuth = () => {
        const token = localStorage.getItem("authToken");
        setIsAuthenticated(!!token);
        if (token) {
          setRole(getUserRole());
          setEmail(getUserEmail());
        } else {
          setRole(null);
          setEmail(null);
        }
      };
  
      checkAuth();
    }, [location]);
  
    const getRatingColor = (rating) => {
      if (rating >= 4.5) return "green";
      if (rating >= 3.5) return "yellow";
      if (rating >= 2) return "orange";
      if (rating > 0) return "red";
      return "gray";
    };
  
    return (
<Card.Root 
  bg="white"
  color="gray.800"
  boxShadow="lg"
  borderRadius="2xl"
  border="1px solid"
  borderColor="gray.200"
  p={6}
  mt={8}
  maxW="5xl" // шире карточка
  mx="auto"
>
  {/* Основной горизонтальный layout */}
  <HStack align="start" spacing={6}>
    {/* Аватарка слева */}
    <Avatar.Root boxSize="120px" flexShrink={0}>
      <Avatar.Image
        src={review.avatar || "https://via.placeholder.com/150"}
      />
      <Avatar.Fallback name={review.nameAuthor || "Аноним"} />
    </Avatar.Root>

    {/* Контент справа */}
    <VStack align="start" spacing={4} w="100%">
      {/* Имя и дата */}
      <Box>
        <Text fontWeight="bold" fontSize="xl">
          {review.nameAuthor || "Аноним"}
        </Text>
        <Text color="gray.500" fontSize="sm">
          {dayjs(review.dateOfCreation).format("DD.MM.YYYY")}
        </Text>
      </Box>

      {/* Оценка */}
      <Box display="flex" alignItems="center">
        <Text fontWeight="medium" color="gray.600" mr={4}>
          Оценка:
        </Text>
        <RatingGroup.Root
          name="rating"
          readOnly
          count={5}
          size="md"
          value={review.theQualityOfTheTransaction ?? 0}
          colorPalette={getRatingColor(review.theQualityOfTheTransaction)}
        >
          <RatingGroup.HiddenInput />
          <RatingGroup.Control />
        </RatingGroup.Root>
      </Box>

      {/* Комментарий */}
      <Box w="100%">
        <Text fontWeight="medium" color="gray.600" mb={1}>
          Комментарий:
        </Text>
        <Text
          fontSize="md"
          color="gray.800"
          px={3}
          py={2}
          bg="gray.50"
          borderRadius="md"
        >
          {review.comment || "Комментарий не указан"}
        </Text>
      </Box>
    </VStack>
  </HStack>
  <HStack mt = "4">                    
    <Button
                      variant="solid"
                      bg="black"
                      color="white"
                      w="50%"
                      size="md"
                      _hover={{ bg: "blue.600" }}
                      onClick={() => {
                        try {
                          navigate(`/user/${review.idAuthorReview}`);
                        } catch (error) {
                          console.error("Ошибка при переходе к владельцу:", error);
                        }
                      }}
                    >
                      К автору
       </Button>      
       <Button
                      variant="solid"
                      bg="black"
                      color="white"
                      w="50%"
                      size="md"
                      _hover={{ bg: "blue.600" }}
                      onClick={() => {
                        try {
                          navigate(`/rentalRequest/${review.idNeedRentalRequest}`);
                        } catch (error) {
                          console.error("Ошибка при переходе к владельцу:", error);
                        }
                      }}
                    >
                      Сделка
       </Button> 
                    </HStack>
                    <HStack mt = "4">   
                                  <Button
                      variant="solid"
                      bg="green"
                      mt = "4"
                      color="white"
                      w="50%"
                      size="md"
                      _hover={{ bg: "blue.600" }}
                      onClick={() => {
                        try {
                          navigate(`/rentalRequest/${review.idNeedRentalRequest}`);
                        } catch (error) {
                          console.error("Ошибка при переходе к владельцу:", error);
                        }
                      }}
                    >
                      Разместить
       </Button> 
                                         <Button
                      variant="solid"
                      //bg="black"
                      mt = "4"
                      color="white"
                      w="50%"
                      size="md"
                      bg = "red"
                      _hover={{ bg: "blue.600" }}
                      onClick={() => {
                        try {
                          navigate(`/rentalRequest/${review.idNeedRentalRequest}`);
                        } catch (error) {
                          console.error("Ошибка при переходе к владельцу:", error);
                        }
                      }}
                    >
                      Отклонить
       </Button> 
       
       </HStack>
</Card.Root>
    );
  }