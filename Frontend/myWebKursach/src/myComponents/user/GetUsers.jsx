import { useEffect, useState } from "react"; 
import { getUsers } from "../../services/users";
import UserCard from "./UserCard";
import { Box, Grid, Text, HStack } from "@chakra-ui/react";
import {
  PaginationItems,
  PaginationNextTrigger,
  PaginationPrevTrigger,
  PaginationRoot,
} from "@/components/ui/pagination";

export default function GetUsers() {
  const [users, setUsers] = useState([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  // Функция для загрузки данных пользователей
  const fetchData = async (page) => {
    try {
      const usersData = await getUsers(page);

      setUsers(usersData.viewModels); 
      setTotalPages(usersData.totalPages);
    } catch (error) {
      console.error("Ошибка при получении данных пользователей:", error);
    }
  };

  // Загружаем пользователей при изменении страницы
  useEffect(() => {
    fetchData(page);
  }, [page]);

  return (
    <Box w="100%" p={0.2}>
      {users.length > 0 ? (
        <>
          <Grid 
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
            {users.map((user) => (
              <UserCard key={user.id} user={user} />
            ))}
          </Grid>

          {/* Пагинация */}
          <PaginationRoot 
          count={totalPages}
          value={page} // currentPage <-- добавлено
            pageSize={1}
            onPageChange={(e) => {
              setPage(e.page)
            }} 
          >
            <HStack justifyContent="center" mt={4}>
              <PaginationPrevTrigger />
              <PaginationItems />
              <PaginationNextTrigger />
            </HStack>
          </PaginationRoot>
        </>
      ) : (
        <Text textAlign="center">Пользователей пока нет</Text>
      )}
    </Box>
  );
}