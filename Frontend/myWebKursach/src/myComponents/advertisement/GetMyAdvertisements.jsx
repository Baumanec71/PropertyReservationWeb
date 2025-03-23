import { useEffect, useState } from "react";
import { getMyAdvertisements } from "../../services/advertisements/getMyAdvertisements";
import { BiLogoBaidu, BiLoaderCircle} from "react-icons/bi";
import AdvertisementCard from "./AdvertisementCard";
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
  WrapItem,
} from "@chakra-ui/react";
import {
  PaginationItems,
  PaginationNextTrigger,
  PaginationPrevTrigger,
  PaginationRoot,
} from "@/components/ui/pagination";
import { useNavigate } from "react-router-dom";

export default function GetMyAdvertisements() {
    const [advertisements, setAdvertisements] = useState([]);
    const navigate = useNavigate();
    const [page, setPage] = useState(1);
  
    const [totalPages, setTotalPages] = useState(1);
    const [isFilterOpen, setIsFilterOpen] = useState(false);
    const [objectTypeLocal, setObjectTypeLocal] = useState("");
    const [renderKey, setRenderKey] = useState(0);
    const { page: routePage } = useParams();  // получаем номер страницы из URL
  // Функция для загрузки данных пользователей




  const fetchData = async (page) => {
    try {
      const advertisements = await getMyAdvertisements(page);

      setAdvertisements(advertisements.viewModels); 
      setTotalPages(advertisements.totalPages);
    } catch (error) {
      console.error("Ошибка при получении данных пользователей:", error);
    }
  };

  useEffect(() => {
    fetchData();
  }, [page]);
  return (
    <Box w="100%" p={4} maxW="1200px" mx="auto" textAlign="center">
   {advertisements.length > 0 ? (
        <>
<Grid
  templateColumns={{
    base: "repeat(auto-fill, minmax(120px, 1fr))",  // для самых маленьких экранов
    sm: "repeat(auto-fill, minmax(220px, 1fr))",     // для небольших экранов
    md: "repeat(auto-fill, minmax(260px, 1fr))",     // для средних экранов
    lg: "repeat(auto-fill, minmax(350px, 1fr))",     // для больших экранов
  }}
  gridAutoRows="1fr"
  gap={6}              // общий промежуток между элементами
  rowGap={8}           // расстояние между строками
  alignItems="stretch" 
  w="100%"
>
            {advertisements.map((advertisement) => (
              <AdvertisementCard key={advertisement.id} ad={advertisement} />
            ))}
          </Grid>
          <PaginationRoot
            count={totalPages}
            value={page}
            pageSize={1}
            onPageChange={(e) => handlePageChange(e.page)}
          >
            <HStack justifyContent="center" mt={4}>
              <PaginationPrevTrigger />
              <PaginationItems />
              <PaginationNextTrigger />
            </HStack>
          </PaginationRoot>
        </>
      ) : (
        <Text textAlign="center" mt={4}>
          Объявлений пока нет
        </Text>
      )}
    </Box>
  );
}