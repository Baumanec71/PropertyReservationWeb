import { Card, Stack, Text, Avatar, Button, Box, RatingGroup } from "@chakra-ui/react";
import { getUserRole } from "../../services/InfoJwt/getUserRole";
import { getUserEmail } from "../../services/InfoJwt/getUserEmail";
import { BrowserRouter as Router, Route, Routes, Link, useLocation, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import dayjs from "dayjs";
export default function UserCard({ user, onEdit }) {
    const [role, setRole] = useState(null);
    const [email, setEmail] = useState(null);
    const navigate = useNavigate();
    const [isAuthenticated, setIsAuthenticated] = useState(false);
       useEffect(() => {
          const checkAuth = () => {
            const token = localStorage.getItem("authToken");
            console.log(token);
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

        const InfoItem = ({ label, value }) => (
          <Box display="flex" justifyContent="space-between" w="100%">
            <Text fontWeight="medium" color="gray.600">{label}:</Text>
            <Text fontWeight="semibold" color="gray.800">{value}</Text>
          </Box>
        );

  return (
<Card.Root
  bg="white"
  color="gray.800"
  boxShadow="lg"
  borderRadius="2xl"
  border="1px solid"
  borderColor="gray.200"
  p={6}
  mt={10}
  maxW="lg"
  mx="auto"
>
  <Card.Header textAlign="center" mb={4}>
    <Card.Title fontSize="2xl" fontWeight="bold" color="teal.600">
      {user.name}
    </Card.Title>
  </Card.Header>

  <Card.Body>
    <Stack spacing={4} align="center" w="full">
      <Avatar.Root boxSize="200px" mb={2} border="2px solid #FDD835" borderRadius="full">
        <Avatar.Image src={user.avatar || "https://via.placeholder.com/150"} />
        <Avatar.Fallback name={user.name} />
      </Avatar.Root>

      <Stack spacing={2} w="full" fontSize="md">
        <InfoItem label="ID" value={user.id} />
        <InfoItem label="Почта" value={user.email} />
        <InfoItem label="Роль" value={user.role} />
        <InfoItem label="Телефон" value={user.phoneNumber} />

        <Box display="flex" alignItems="center">
          <Text  fontWeight="medium" color="gray.600" mr={24}>Рейтинг:</Text>
          <RatingGroup.Root
            name="Рейтинг"
            placeholder="Рейтинг:"
            readOnly
            count={5}
            size="md"
            value={user.rating ?? 0}
            colorPalette={getRatingColor(user.rating ?? 0)}
            defaultValue={0}
          >
            <RatingGroup.HiddenInput />
            <RatingGroup.Control />
          </RatingGroup.Root>
        </Box>

        <InfoItem label="Созданных объявлений" value={user.numberOfAdsCreated} />
        <InfoItem label="Количество сделок" value={user.numberOfTransactions} />
        <InfoItem label="Статус" value={user.deleteStatus ? "Заблокирован" : "Активный"} />
        <InfoItem label="Дата регистрации" value={dayjs(user.dateOfRegistration).format("DD.MM.YYYY")} />
      </Stack>
    </Stack>
  </Card.Body>
      <Card.Footer justifyContent="flex-end">
  {!isAuthenticated ? null : (
    <Box w="100%">
      {role === "Admin" && (
        <Button
          variant="solid"
          bg="red.600"
          w="100%"
          px={6}
          py={3}
          rounded="lg"
          _hover={{ color: "black", bg: "red.600" }}
          onClick={() => navigate(`/DeleteUserForAdmin/${ad.id}`)}
        >
          Заблокировать Пользователя
        </Button>
      )}
      {email === user.email && (
        <>
          <Button
        onClick={onEdit}
        width="full"
        size="lg"
        px={6}
        py={3}
        mt={4}
        bg="#111111"
        //bg="#FFEB3B"
        color="white"
        fontWeight="semibold"
        rounded="xl"
       
        transition="all 0.3s ease"
        _hover={{
          bg: "#FDD835", // чуть темнее при наведении
          transform: "scale(1.04)",
          color: "black",
          boxShadow: "0 6px 14px rgba(253, 216, 53, 0.35)"
        }}
        _active={{
          transform: "scale(0.98)",
          boxShadow: "0 2px 6px rgba(253, 216, 53, 0.2)"
        }}
      >
        Редактировать
      </Button>
      <Button  onClick={() => navigate(`/change-password`)}   w="100%"
      mt={2}
  px={4}
  py={2}
  rounded="lg"
  variant="outline"
  borderColor="#E0E0E0"
  color="#111111"
  fontWeight="medium"
  _hover={{
    bg: "#FFF9C4",
    transform: "scale(1.03)"
  }}
  transition="all 0.2s">Смена пароля</Button>
        </>
      )}
    </Box>
  )}
</Card.Footer>
    
    </Card.Root>
  );
}
