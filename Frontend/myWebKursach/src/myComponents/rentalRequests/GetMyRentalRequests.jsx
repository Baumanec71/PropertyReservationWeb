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
  WrapItem,
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
import RentalRequestCard from "./RentalRequestCard";
import { getMyRentalRequests } from "@/services/rentalRequests/getMyRentalRequests";

export default function GetMyRentalRequests() {
  const [rentalRequests, setRentalRequests] = useState([]);
  const navigate = useNavigate();
  const [page, setPage] = useState(1);

  const [totalPages, setTotalPages] = useState(1);
  const { page: routePage } = useParams();

  const fetchData = async () => {
    try {
      const rentalRequestsData = await getMyRentalRequests(page);
      if (rentalRequestsData?.viewModels) {
        setRentalRequests(rentalRequestsData.viewModels);
        setTotalPages(rentalRequestsData.totalPages || 1);
      } else {
        setRentalRequests([]);
        setTotalPages(1);
      }
    } catch (error) {
      console.error("Ошибка при получении данных объявлений:", error);
      setRentalRequests([]);
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

  // Применить фильтры
  const handleApplyFilters = () => {
    setPage(1);
    fetchData(filterModel);
  };

  const handlePageChange = (newPage) => {
    setPage(newPage);
    navigate(`/myRentalRequests/${newPage}`);  // переход по маршруту с номером страницы
  };


  return (
    <Box w="100%" p={4} maxW="1200px" mx="auto" textAlign="center">
      {rentalRequests.length > 0 ? (
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
            {rentalRequests.map((rentalRequest) => (
            <RentalRequestCard key={rentalRequest.id} request={rentalRequest} o onEdit={fetchData} />
           //   <RentalRequestCard key={rentalRequest.id} request={rentalRequest} />
            ))}
          </Grid>
          <PaginationRoot
            color = "black"
            count={totalPages}
            value={page}
            pageSize={1}
            onPageChange={(e) => handlePageChange(e.page)}
          >
            <HStack justifyContent="center" mt={4}>
              <PaginationPrevTrigger color = "black"/>
              <PaginationItems color = "black"/>
              <PaginationNextTrigger color = "black"/>
            </HStack>
          </PaginationRoot>
        </>
      ) : (
        <Text color = "black" textAlign="center" mt={4}>
          запросов на аренду пока нет
        </Text>
      )}
    </Box>
  );
}