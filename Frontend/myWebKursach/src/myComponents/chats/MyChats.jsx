import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  Box,
  Grid,
  Text,
  HStack,
} from "@chakra-ui/react";
import {
  PaginationItems,
  PaginationNextTrigger,
  PaginationPrevTrigger,
  PaginationRoot,
} from "@/components/ui/pagination";
import ChatCard from "./ChatCard";

// Заглушка чатов (демо-данные)
const dummyChats = Array.from({ length: 17 }).map((_, index) => ({
  id: index + 1,
  participant1Id: `user_${(index % 5) + 1}`,
  participant2Id: `user_${(index % 7) + 6}`,
  createdAt: new Date(Date.now() - index * 10000000).toISOString(),
}));

const PAGE_SIZE = 6;

const MyChats = () => {
  const navigate = useNavigate();
  const { page: routePage } = useParams();
  const [page, setPage] = useState(1);
  const [chats, setChats] = useState([]);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    setPage(Number(routePage) || 1);
  }, [routePage]);

  useEffect(() => {
    fetchData();
  }, [page]);

  const fetchData = () => {
    const start = (page - 1) * PAGE_SIZE;
    const end = start + PAGE_SIZE;
    setChats(dummyChats.slice(start, end));
    setTotalPages(Math.ceil(dummyChats.length / PAGE_SIZE));
  };

  const handlePageChange = (newPage) => {
    setPage(newPage);
    navigate(`/my-chats/${newPage}`);
  };

  return (
    <Box w="100%" p={4} maxW="1200px" mx="auto" textAlign="center">
      {chats.length > 0 ? (
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
            {chats.map((chat) => (
              <ChatCard key={chat.id} chat={chat} />
            ))}
          </Grid>
          <PaginationRoot
            color="black"
            count={totalPages}
            value={page}
            pageSize={1}
            onPageChange={(e) => handlePageChange(e.page)}
          >
            <HStack justifyContent="center" mt={4}>
              <PaginationPrevTrigger color="black" />
              <PaginationItems color="black" />
              <PaginationNextTrigger color="black" />
            </HStack>
          </PaginationRoot>
        </>
      ) : (
        <Text color="black" textAlign="center" mt={4}>
          чатов нет
        </Text>
      )}
    </Box>
  );
};

export default MyChats;