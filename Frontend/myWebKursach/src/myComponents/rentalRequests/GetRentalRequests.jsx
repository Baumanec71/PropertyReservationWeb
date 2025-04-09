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
import { getRentalRequests } from "@/services/rentalRequests/getRentalRequests";

export default function GetRentalRequests() {
  const [rentalRequests, setRentalRequests] = useState([]);
  const { idNeedAdvertisement } = useParams();
  const navigate = useNavigate();
  const [page, setPage] = useState(1);

  const [totalPages, setTotalPages] = useState(1);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [approvalStatusLocal, setApprovalStatusLocal] = useState("");
  const [renderKey, setRenderKey] = useState(0);
  const { page: routePage } = useParams();

  const [filterModel, setFilterModel] = useState({
    selectedApprovalStatus: null,
    selectedBookingStartDate: null,
    selectedBookingFinishDate: null,
    selectedDataChangeStatus: null,
    selectedIdNeedAdvertisement: null,
    types: [],
  });

  const handleApprovalStatusChange = (newVal) => {
    setApprovalStatusLocal(newVal.value);
    setFilterModel((prev) => ({
      ...prev,
      selectedApprovalStatus: newVal.value === "" ? null : Number(newVal.value),
    }));
  };

  const approvalStatusCollection = createListCollection({
    items: filterModel.types.length
      ? filterModel.types.map((option) => ({
          label: option.displayName,
          value: String(option.value),
        }))
      : [
          { label: "Квартира", value: "0" },
          { label: "Дом", value: "1" },
        ],
  });

  const fetchData = async (filters) => {
    try {
      console.log("Фильтры перед запросом:", filters);
      const rentalRequestsData = await getRentalRequests(page, idNeedAdvertisement, filters);
      if (rentalRequestsData?.viewModels) {
        setRentalRequests(rentalRequestsData.viewModels);
        setTotalPages(rentalRequestsData.totalPages || 1);
        setFilterModel((prev) => ({
          ...prev,
          types: rentalRequestsData.filterModel?.types || prev.types,   
        }));
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

  // Сброс фильтров
  const handleResetFilters = () => {
    const newFilterModel = {
        selectedApprovalStatus: null,
        selectedBookingStartDate: null,
        selectedBookingFinishDate: null,
        selectedDataChangeStatus: null,
        selectedIdNeedAdvertisement: null,
        types: [],
    };
    // Устанавливаем сброшенный фильтр
    setFilterModel(newFilterModel);
    setRenderKey((prev) => prev + 1);

    setApprovalStatusLocal("");
    setPage(1);
    fetchData(newFilterModel);
  };

  const handleFilterChange = (e) => {
    const { name, value, type } = e.target;
    setFilterModel((prev) => ({
      ...prev,
      [name]: value === "" ? null : type === "number" ? Number(value) : value,
    }));
  };

  const handlePageChange = (newPage) => {
    setPage(newPage);
    navigate(`/rentalRequests/${idNeedAdvertisement}/${newPage}`);  // переход по маршруту с номером страницы
  };


  return (
    <Box w="100%" p={4} maxW="1200px" mx="auto" textAlign="center">
      {/* Верхняя панель поиска */}
      <HStack w="100%" spacing={2} mb={4} justify="center" >
        <Button
          onClick={() => setIsFilterOpen(!isFilterOpen)}
          className="px-4 py-2 bg-blue-500 text-white rounded-lg shadow-md"
        >
          Фильтр
        </Button>
      </HStack>

      {/* Фильтр */}
      <Collapsible.Root open={isFilterOpen}>
  <Collapsible.Trigger />
  <Collapsible.Content>
    <VStack spacing={5} p={5} bg="gray.50" rounded="lg" boxShadow="md">
      <HStack w="100%" spacing={4}>
        <SelectRoot
          collection={approvalStatusCollection}
          value={Array.isArray(approvalStatusLocal) ? approvalStatusLocal : [approvalStatusLocal]} 
          onValueChange={handleApprovalStatusChange}
          size="sm"
          width="full"
        >
          <SelectLabel fontWeight="semibold" color="gray.700">Статус бронирования</SelectLabel>
          <SelectTrigger borderColor="gray.300" _hover={{ borderColor: "blue.500" }}>
            <SelectValueText placeholder="Выберите тип объекта" />
          </SelectTrigger>
          <SelectContent>
            {approvalStatusCollection.items.map((item) => (
              <SelectItem key={item.value} item={item}>
                {item.label}
              </SelectItem>
            ))}
          </SelectContent>
        </SelectRoot>
      </HStack>
      <HStack w="100%" justifyContent="space-between" spacing={4}>
        <Button
          onClick={handleApplyFilters}
          bg="blue.500"
          color="white"
          px={6}
          py={3}
          rounded="lg"
          _hover={{ bg: "blue.600" }}
        >
          Поиск
        </Button>
        <Button
          onClick={handleResetFilters}
          bg="gray.400"
          color="white"
          px={6}
          py={3}
          rounded="lg"
          _hover={{ bg: "gray.500" }}
        >
          Сбросить
        </Button>
      </HStack>
    </VStack>
  </Collapsible.Content>
</Collapsible.Root>
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
              <RentalRequestCard key={rentalRequest.id} request={rentalRequest} />
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
          запросов на аренду пока нет
        </Text>
      )}
    </Box>
  );
}