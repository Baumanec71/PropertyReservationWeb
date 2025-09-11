import { Card, Stack, Text, Avatar, Button, Box, RatingGroup, Heading } from "@chakra-ui/react";
import { getUserRole } from "../../services/InfoJwt/getUserRole";
import { getUserEmail } from "../../services/InfoJwt/getUserEmail";
import { BrowserRouter as Router, Route, Routes, Link, useLocation, useNavigate } from "react-router-dom";
//import { ChatConversation } from "./ChatConversation";
import { useState, useEffect } from "react";
import dayjs from "dayjs";
export default function ChatCard({ chat, onEdit }) {
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
      {"bob"}
    </Card.Title>
  </Card.Header>

  <Card.Body>
  <Stack spacing={4} align="center" w="full">
      {/*  <Heading fontSize="2xl" color="teal.600">
            Пользователь {chat.participant2Id}
        </Heading> */}
        <Text fontSize="md" color="gray.600">
          Дата создания: {dayjs(chat.createdAt).format("DD.MM.YYYY")}
        </Text>
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
      {(
        <>
                  <Button
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
        onClick={() => navigate(`/ChatConversation`)}
        
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
        Перейти в чат
      </Button>
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
        Удалить
      </Button>
        </>
      )}
    </Box>
  )}
</Card.Footer>
    
    </Card.Root>
  );
}
