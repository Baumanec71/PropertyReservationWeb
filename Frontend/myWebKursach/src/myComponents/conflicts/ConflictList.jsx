import React, { useEffect, useState } from "react";
import axios from "axios";
import {
  Card,
  CardHeader,
  CardBody,
  CardFooter,
  Heading,
  Text,
  Button,
  HStack,
  Box,
  Tag,
} from "@chakra-ui/react";
import { useNavigate, useLocation } from "react-router-dom";
const getConflicts = async (page = 1) => {
  try {
    const token = localStorage.getItem("authToken");
    if (!token) {
      return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
    }

    const config = {
      headers: {
        Authorization: `Bearer ${token}`,
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      withCredentials: true, // Для работы с cookies, если необходимо
    };

    // Запрос на получение данных о конфликтах
    const response = await axios.get(
      `${API_BASE_URL}/api/Conflict/GetConflicts?page=${page}`,
      config
    );

    if (response.status === 200) {
      return { success: true, data: response.data };
    } else {
      return { success: false, error: response.data.Description || "Неизвестная ошибка" };
    }
  } catch (error) {
    console.error("Ошибка при получении конфликтов:", error);
    return { success: false, error: error.message || "Что-то пошло не так" };
  }
};

const handleConflictAction = async (action, conflictId) => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        return alert("Токен отсутствует, авторизуйтесь снова.");
      }
  
      const config = {
        headers: {
          Authorization: `Bearer ${token}`,
          Accept: "application/json",
          "Content-Type": "application/json",
        },
      };
  
      const endpoint =
        action === "reject"
          ? `${API_BASE_URL}/api/Conflict/RejectedConflict?id=${conflictId}`
          : `${API_BASE_URL}/api/Conflict/ResolvedConflict?id=${conflictId}`;
  
      const response = await axios.post(endpoint, {}, config);
  
      if (response.status === 200) {
        alert(action === "reject" ? "Конфликт отклонен." : "Конфликт разрешен.");
      } else {
        alert(response.data.Description || "Ошибка при изменении статуса конфликта.");
      }
    } catch (error) {
      console.error("Ошибка при изменении статуса конфликта:", error);
      alert("Произошла ошибка при выполнении запроса.");
    }
  };

export default function ConflictList() {
  const [conflicts, setConflicts] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const fetchConflicts = async () => {
      const result = await getConflicts();
      if (result.success) {
        setConflicts(result.data);
      } else {
        setError(result.error);
      }
      setLoading(false);
    };

    fetchConflicts();
  }, []);

  if (loading) return <Text>Загрузка...</Text>;
  if (error) return <Text color="red.500">{error}</Text>;

  return (
    <Box mt="4">
      {Array.isArray(conflicts.viewModels) && conflicts.viewModels.length > 0 ? (
        conflicts.viewModels.map((conflict) => (
          <Card.Root key={conflict.id} mb={4} borderWidth={1} borderRadius="md" boxShadow="sm">
            <Card.Header>
              <Heading size="md">{`Конфликт #${conflict.id}`}</Heading>
            </Card.Header>
            <Card.Body>
              <Text mb={2}><strong>Описание:</strong> {conflict.description}</Text>
              <Text mb={2}><strong>Статус:</strong> <Tag.Root>
                  <Tag.Label>{conflict.status}</Tag.Label>
                </Tag.Root></Text>
              <Text mb={2}><strong>Дата создания:</strong> {new Date(conflict.dateCreated).toLocaleString()}</Text>
              {conflict.dateResolved && (
                <Text><strong>Дата разрешения:</strong> {new Date(conflict.dateResolved).toLocaleString()}</Text>
              )}
            </Card.Body>
            <CardFooter>
              <HStack spacing={4}>
                <Button
                  bg="red"
                  w="30%"
                  onClick={() => handleConflictAction("reject", conflict.id)}
                >
                  Отклонить
                </Button>
                <Button
                  bg="green"
                  w="30%"
                  onClick={() => handleConflictAction("resolve", conflict.id)}
                >
                  Решен
                </Button>
                <Button
                      variant="solid"
                      bg="black"
                      color="white"
                      w="40%"
                      size="md"
                      _hover={{ bg: "blue.600" }}
                      onClick={() => {
                        try {
                          navigate(`/rentalRequest/${conflict.rentalRequestId}`);
                        } catch (error) {
                          console.error("Ошибка при переходе к владельцу:", error);
                        }
                      }}
                    >
                      Сделка
                </Button> 
               
              </HStack>
            </CardFooter>
          </Card.Root>
        ))
      ) : (
        <Text>Нет данных для отображения</Text>
      )}
    </Box>
  );
}