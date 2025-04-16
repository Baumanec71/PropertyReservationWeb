import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Grid,
  Text,
  HStack,
  VStack,
  Input,
  Button,
  Select,
  Checkbox,
  CheckboxGroup,
  Stack,
  Collapsible,
  RatingGroup,
  GridItem,
  createListCollection,
  Wrap,
  WrapItem, Flex
} from "@chakra-ui/react";
import {
  PaginationItems,
  PaginationNextTrigger,
  PaginationPrevTrigger,
  PaginationRoot,
} from "@/components/ui/pagination";
import {
  SelectContent,
  SelectItem,
  SelectLabel,
  SelectRoot,
  SelectTrigger,
  SelectValueText,
} from "@/components/ui/select";
import { useNavigate } from "react-router-dom";
import ReviewCard from "./ReviewCard";
import { getReviewsForAdvertisement } from "@/services/reviews/getReviewsForAdvertisement";

export default function GetReviewForAdvertisement() {
  const [reviews, setReviews] = useState([]);
  const { idAdvertisement } = useParams();
  const navigate = useNavigate();
  const [page, setPage] = useState(1);

  const [totalPages, setTotalPages] = useState(1);
  const { page: routePage } = useParams();
console.log(idAdvertisement)
  const fetchData = async () => {
    try {
      const reviewsData = await getReviewsForAdvertisement(idAdvertisement, page);
      if (reviewsData?.viewModels) {
        setReviews(reviewsData.viewModels);
        setTotalPages(reviewsData.totalPages || 1);
      } else {
        setReviews([]);
        setTotalPages(1);
      }
    } catch (error) {
      console.error("Ошибка при получении данных о отзывах на объявление:", error);
      setReviews([]);
      setTotalPages(1);
    }
  };

  useEffect(() => {
    fetchData();
  }, [page]);

  useEffect(() => {
    // Если номер страницы в URL существует, используем его
    setPage(Number(routePage) || 1);
  }, [routePage]);


  const handlePageChange = (newPage) => {
    setPage(newPage);
    navigate(`/reviews/${idAdvertisement}/${newPage}`);  // переход по маршруту с номером страницы
  };


  return (
    <Flex
    direction="column"
    minH="100vh"
    px={4}
    maxW="1200px"
    mx="auto"
    w="100%"
  >
    {reviews.length > 0 ? (
      <>
        {/* Контейнер с карточками */}
        <Box flex="1" py={6}>
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
            {reviews.map((review) => (
              <ReviewCard key={review.id} review={review} onEdit={fetchData} />
            ))}
          </Grid>
        </Box>
  
        {/* Пагинация снизу */}
        <Box mt={8} mb={4}>
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
        </Box>
      </>
    ) : (
      <Text color="black" textAlign="center" mt={4} flex="1">
        Отзывов пока нет
      </Text>
    )}
  </Flex>
  );
}