import { Card, Stack, Text, Avatar, Button, Box } from "@chakra-ui/react";
import { getUserRole } from "../../services/InfoJwt/getUserRole";
import { getUserEmail } from "../../services/InfoJwt/getUserEmail";
import { useState, useEffect } from "react";
export default function UserCard({ user, onEdit }) {
    const [role, setRole] = useState(null);
    const [email, setEmail] = useState(null);
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
  return (
    <Card.Root  bg= "white" color="black">
      <Card.Header>
        <Card.Title>{user.name}</Card.Title>
      </Card.Header>
      <Card.Body>
        <Stack gap="4" w="full" align="center">
        <Avatar.Root boxSize="100px"  mb={4} pos={"relative"}  >
          <Avatar.Image src={user.avatar || "https://via.placeholder.com/150"} />
          <Avatar.Fallback name={user.name} />
        </Avatar.Root>
          <Text><b>ID:</b> {user.id}</Text>
          <Text><b>Почта:</b> {user.email}</Text>
          <Text><b>Роль:</b> {user.role}</Text>
          <Text><b>Номер телефона:</b> {user.phoneNumber}</Text>
          <Text><b>Рейтинг:</b> {user.rating}</Text>
          <Text><b>Созданных объявлений:</b> {user.numberOfAdsCreated}</Text>
          <Text><b>Количество сделок:</b> {user.numberOfTransactions}</Text>
          <Text><b>Статус:</b> {user.deleteStatus ? "Заблокирован" : "Активный"}</Text>
          <Text><b>Дата регистрации:</b> {user.dateOfRegistration}</Text>
        </Stack>
      </Card.Body>
      <Card.Footer justifyContent="flex-end">
  {!isAuthenticated ? null : (
    <Box w="100%">
      {role === "Admin" && (
        <Button
          variant="solid"
          bg="red"
          w="100%"
          px={6}
          py={3}
          rounded="lg"
          _hover={{ bg: "red.600" }}
          onClick={() => navigate(`/DeleteUserForAdmin/${ad.id}`)}
        >
          Удалить Пользователя
        </Button>
      )}
      {email === user.email && (
        <Button
          variant="solid"
          bg="green"
          _hover={{ bg: "green.600" }}
          w="100%"
          px={6}
          py={3}
          rounded="lg"
          color = "white"
          mt={4} // Добавляет отступ сверху
          onClick={onEdit}
        >
          Редактировать
        </Button>
      )}
    </Box>
  )}
</Card.Footer>
    
    </Card.Root>
  );
}
