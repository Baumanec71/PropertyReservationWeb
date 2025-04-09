import { useEffect, useState } from "react";
import { getAllAdvertisements } from "../../services/advertisements/getAllAdvertisements";
import { BiLogoBaidu, BiLoaderCircle} from "react-icons/bi";
import AdvertisementCard from "./AdvertisementCard";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";

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


export default function GetAllAdvertisements() {
  const [advertisements, setAdvertisements] = useState([]);
  const navigate = useNavigate();
  const [page, setPage] = useState(1);

  const [totalPages, setTotalPages] = useState(1);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [objectTypeLocal, setObjectTypeLocal] = useState("");
  const [renderKey, setRenderKey] = useState(0);
  const { page: routePage } = useParams();  // получаем номер страницы из URL

  const [filterModel, setFilterModel] = useState({
    selectedObjectType: null,
    selectedAddress: null,
    selectedMinRentalPrice: null,
    selectedMaxRentalPrice: null,
    selectedMinFixedPrepaymentAmount: null,
    selectedMaxFixedPrepaymentAmount: null,
    selectedNumberOfRooms: null,
    selectedNumberOfBeds: null,
    selectedNumberOfBathrooms: null,
    selectedMinRating: null,
    selectedConfirmationStatus: null,
    selectedDeleteStatus: null,
    createAdvertisementAmenities: [],
    types: [],
  });

  const handleCheckboxChange = (amenity, checked) => {
    console.log("Изменение чекбокса:", amenity, checked);
    setFilterModel((prev) => ({
      ...prev,
      createAdvertisementAmenities: prev.createAdvertisementAmenities.map((a) =>
        a.amenity === amenity.amenity
          ? {
              ...a,
              isActive: checked,
              value: checked && ![2, 3].includes(a.amenity) ? 1 : a.value,
            }
          : a
      ),
    }));
  };

  const handleObjectTypeChange = (newVal) => {
    setObjectTypeLocal(newVal.value);
    setFilterModel((prev) => ({
      ...prev,
      selectedObjectType: newVal.value === "" ? null : Number(newVal.value),
    }));
  };

  const objectTypeCollection = createListCollection({
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
      const advertisementsData = await getAllAdvertisements(page, filters);
      if (advertisementsData?.viewModels) {
        setAdvertisements(advertisementsData.viewModels);
        setTotalPages(advertisementsData.totalPages || 1);
        setFilterModel((prev) => ({
          ...prev,
          types: advertisementsData.filterModel?.types || prev.types,
          createAdvertisementAmenities:
            filters && filters.createAdvertisementAmenities
              ? filters.createAdvertisementAmenities
              : advertisementsData.filterModel?.createAdvertisementAmenities ||
                prev.createAdvertisementAmenities,
        }));
      } else {
        setAdvertisements([]);
        setTotalPages(1);
      }
    } catch (error) {
      console.error("Ошибка при получении данных объявлений:", error);
      setAdvertisements([]);
      setTotalPages(1);
    }
  };

  useEffect(() => {
    fetchData();
  }, [page]);

  useEffect(() => {
    setPage(Number(routePage) || 1);
  }, [routePage]);

  const handleApplyFilters = () => {
    setPage(1);
    fetchData(filterModel);
  };

  const handleResetFilters = () => {
    const newFilterModel = {
      selectedObjectType: null,
      selectedAddress: null,
      selectedMinRentalPrice: null,
      selectedMaxRentalPrice: null,
      selectedMinFixedPrepaymentAmount: null,
      selectedMaxFixedPrepaymentAmount: null,
      selectedNumberOfRooms: null,
      selectedNumberOfBeds: null,
      selectedNumberOfBathrooms: null,
      selectedMinRating: null,
      selectedConfirmationStatus: null,
      selectedDeleteStatus: null,
      createAdvertisementAmenities: filterModel.createAdvertisementAmenities.map((a) => ({
        ...a,
        isActive: false,
      })),
      types: [],
    };
    // Устанавливаем сброшенный фильтр
    setFilterModel(newFilterModel);

    // Инкрементируем renderKey, чтобы CheckboxGroup пересоздался
    setRenderKey((prev) => prev + 1);

    setObjectTypeLocal("");
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
    navigate(`/Advertisements/${newPage}`);  // переход по маршруту с номером страницы
  };

  return (
    <Box w="100%" p={4} maxW="1200px" mx="auto" textAlign="center">
      {/* Верхняя панель поиска */}
      <HStack w="100%" spacing={2} mb={4} justify="center" >
        <Input
          name="selectedAddress"
          placeholder="Поиск по адресу"
          value={filterModel.selectedAddress ?? ""}
          onChange={handleFilterChange}
          className="w-3/5 p-2 border border-gray-300 rounded-lg shadow-sm"
        />
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
          collection={objectTypeCollection}
          value={Array.isArray(objectTypeLocal) ? objectTypeLocal : [objectTypeLocal]} 
          onValueChange={handleObjectTypeChange}
          size="sm"
          width="full"
        >
          <SelectLabel fontWeight="semibold" color="gray.700">Тип объекта</SelectLabel>
          <SelectTrigger borderColor="gray.300" _hover={{ borderColor: "blue.500" }}>
            <SelectValueText placeholder="Выберите тип объекта" />
          </SelectTrigger>
          <SelectContent>
            {objectTypeCollection.items.map((item) => (
              <SelectItem key={item.value} item={item}>
                {item.label}
              </SelectItem>
            ))}
          </SelectContent>
        </SelectRoot>

        <RatingGroup.Root
          name="selectedMinRating"
          placeholder="Минимальный рейтинг объявления"
          count={5}
          size="md"
          value={filterModel.selectedMinRating ?? 0}
          onValueChange={(newValue) => {
            const rating = newValue?.value ?? 0;
            setFilterModel((prev) => ({ ...prev, selectedMinRating: rating }));
          }}
        >
          <RatingGroup.HiddenInput />
          <RatingGroup.Control />
        </RatingGroup.Root>
      </HStack>

      <HStack w="100%" spacing={4}>
        <Input
          name="selectedMinRentalPrice"
          type="number"
          placeholder="Цена от"
          value={filterModel.selectedMinRentalPrice ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "blue.500" }}
        />
        <Input
          name="selectedMaxRentalPrice"
          type="number"
          placeholder="Цена до"
          value={filterModel.selectedMaxRentalPrice ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "blue.500" }}
        />
      </HStack>

      <HStack w="100%" spacing={4}>
        <Input
          name="selectedMinFixedPrepaymentAmount"
          type="number"
          placeholder="Предоплата от"
          value={filterModel.selectedMinFixedPrepaymentAmount ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "blue.500" }}
        />
        <Input
          name="selectedMaxFixedPrepaymentAmount"
          type="number"
          placeholder="Предоплата до"
          value={filterModel.selectedMaxFixedPrepaymentAmount ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "blue.500" }}
        />
      </HStack>

      <HStack w="100%" spacing={4}>
        <Input
          name="selectedNumberOfRooms"
          type="number"
          placeholder="Количество комнат"
          value={filterModel.selectedNumberOfRooms ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "blue.500" }}
        />
        <Input
          name="selectedNumberOfBeds"
          type="number"
          placeholder="Число спальных мест"
          value={filterModel.selectedNumberOfBeds ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "blue.500" }}
        />
      </HStack>

      <HStack w="100%" spacing={4}>
  <Input
    name="selectedConfirmationStatus"
    type="text"
    placeholder="Одобренные? (true/false)"
    value={filterModel.selectedConfirmationStatus ?? ""}
    onChange={handleFilterChange}
    borderColor="gray.300"
    _focus={{ borderColor: "blue.500" }}
    pattern="^(true|false)$" // позволяет вводить только "true" или "false"
  />
  <Input
    name="selectedDeleteStatus"
    type="text"
    placeholder="Удаленные? (true/false)"
    value={filterModel.selectedDeleteStatus ?? ""}
    onChange={handleFilterChange}
    borderColor="gray.300"
    _focus={{ borderColor: "blue.500" }}
    pattern="^(true|false)$" // позволяет вводить только "true" или "false"
  />
</HStack>
      <HStack w="100%" spacing={4}>
        <Input
          name="selectedNumberOfBathrooms"
          type="number"
          placeholder="Число ванных комнат"
          value={filterModel.selectedNumberOfBathrooms ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "blue.500" }}
        />
      </HStack>
      

      {/* Группа чекбоксов */}
      
        <CheckboxGroup name="Удобства" key={renderKey}>
          <Grid
            templateColumns={{
              base: "repeat(1, 1fr)", // 1 столбец на мобильных устройствах
              sm: "repeat(2, 1fr)", // 2 столбца на малых экранах
              md: "repeat(3, 1fr)", // 3 столбца на средних экранах
            }}
            gap={4}
            rowGap={6}
            w="100%"
          >
            {(filterModel.createAdvertisementAmenities || []).map((amenity) => (
              <GridItem key={amenity.amenity}>
                <Checkbox.Root
                  value={amenity.amenity}
                  checked={amenity.isActive || false}
                  onCheckedChange={(e) => handleCheckboxChange(amenity, !!e.checked)}
                >
                  <Checkbox.HiddenInput />
                  <Checkbox.Control
                    borderColor="gray.300"
                    _checked={{ bg: "blue.500", borderColor: "blue.500" }}
                    _hover={{ borderColor: "blue.300" }}
                  />
                  <Checkbox.Label>{amenity.amenityDisplay}</Checkbox.Label>
                </Checkbox.Root>
              </GridItem>
            ))}
          </Grid>
        </CheckboxGroup>
     

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

      {/* Список объявлений */}
      {advertisements.length > 0 ? (
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