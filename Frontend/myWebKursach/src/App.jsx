import React, { useEffect, useState } from "react";
import { BrowserRouter as Router, Route, Routes, Link, useLocation, useNavigate } from "react-router-dom";
import { Button, Container, VStack, HStack, Box, Grid, useBreakpointValue, Drawer, Accordion, Text, IconButton, Flex, Icon, SimpleGrid    } from "@chakra-ui/react";
import {
  DrawerRoot,
  DrawerBackdrop,
  DrawerTrigger,
  DrawerContent,
  DrawerHeader,
  DrawerTitle,
  DrawerBody,
  DrawerCloseTrigger,
} from "@/components/ui/drawer";
import { CgProfile } from "react-icons/cg";
import CreateAccount from "./myComponents/CreateAccount";
import GetUsers from "./myComponents/user/GetUsers";
import GetUser from "./myComponents/user/GetUser";
import Profile from "./myComponents/user/Profile";
import Login from "./myComponents/Login";
import ChangePassword from "./myComponents/user/ChangePassword"; 
import { getUserRole } from "./services/InfoJwt/getUserRole";
import { getUserEmail } from "./services/InfoJwt/getUserEmail";
import { logout } from "./services/logout";
import axios from "axios";
import CreateAdvertisement from "./myComponents/advertisement/CreateAdvertisement";
import CreateRentalRequest from "./myComponents/rentalRequests/CreateRentalRequest";
import GetAdvertisements from "./myComponents/advertisement/GetAdvertisements";
import GetMyAdvertisements from "./myComponents/advertisement/GetMyAdvertisements";
import GetAllAdvertisements from "./myComponents/advertisement/GetAllAdvertisements";
import GetAdvertisementDetails from "./myComponents/advertisement/AdvertisementDetails";
import GetRentalRequests from "./myComponents/rentalRequests/GetRentalRequests";
import GetRentalRequest from "./myComponents/rentalRequests/GetRentalRequest";
import GetMyRentalRequests from "./myComponents/rentalRequests/getMyRentalRequests";
import GetMySentRentalRequests from "./myComponents/rentalRequests/getMySentRentalRequests";
import CreateReview from "./myComponents/review/CreateReview";
import GetReviewForAdvertisement from "./myComponents/review/GetReviewForAdvertisement";
import MyChats from "./myComponents/chats/MyChats";
import ChatConversation from "./myComponents/chats/ChatConversation";


import ConflictList from "./myComponents/conflicts/ConflictList";
import GetBookingPhotos from "./myComponents/bookingPhotos/GetBookingPhotos";
import { useColorMode, useColorModeValue } from "@/components/ui/color-mode"
import { FaGithub, FaPhone, FaEnvelope, FaMapMarkerAlt, FaInstagram, FaTelegram, FaFacebook } from 'react-icons/fa';

const Footer = () => {
  return (
    <Box
      as="footer"
      w="100%"
      mt="auto"
      py={10}
      px={8}
      bg="gray.800"
      color="white"
    >
      <SimpleGrid columns={{ base: 1, md: 3 }} spacing={10} mb={6}>
        {/* Контакты */}
        <VStack align="flex-start" spacing={3}>
          <Text fontSize="lg" fontWeight="bold">Контакты</Text>
          <HStack spacing={2}>
            <Icon as={FaPhone} />
            <Text>+7 (800) 555-35-35</Text>
          </HStack>
          <HStack spacing={2}>
            <Icon as={FaEnvelope} />
            <Text>support@nicehome.ru</Text>
          </HStack>
          <HStack spacing={2}>
            <Icon as={FaMapMarkerAlt} />
            <Text>г. Москва, ул. Примерная, д. 1</Text>
          </HStack>
        </VStack>

        {/* Навигация */}
        <VStack align="flex-start" spacing={2}>
          <Text fontSize="lg" fontWeight="bold">Навигация</Text>
          <Link href="/about">О нас</Link>
          <Link href="/faq">Вопросы и ответы</Link>
          <Link href="/contacts">Контакты</Link>
          <Link href="/rent">Сдать жильё</Link>
          <Link href="/rules">Правила сервиса</Link>
        </VStack>

        {/* Соцсети */}
        <VStack align="flex-start" spacing={2}>
          <Text fontSize="lg" fontWeight="bold">Мы в соцсетях</Text>
          <HStack spacing={2}>
            <Icon as={FaTelegram} />
            <Link href="https://t.me/nicehome" isExternal>
              Telegram
            </Link>
          </HStack>
          <HStack spacing={2}>
            <Icon as={FaFacebook} />
            <Link href="https://facebook.com/nicehome" isExternal>
              Facebook
            </Link>
          </HStack>
          <HStack spacing={2}>
            <Icon as={FaGithub} />
            <Link href="https://github.com/your-repo" isExternal>
              GitHub
            </Link>
          </HStack>
        </VStack>
      </SimpleGrid>


      {/* Нижняя строка */}
      <VStack spacing={1} fontSize="sm" textAlign="center">
        <Text>© {new Date().getFullYear()} NiceHome. Все права защищены.</Text>
        <HStack spacing={4} justify="center">
          <Link href="/privacy">Политика конфиденциальности</Link>
          <Link href="/terms">Пользовательское соглашение</Link>
        </HStack>
      </VStack>
    </Box>
  );
};

const Layout = () => {
  const buttonSize = useBreakpointValue({ base: "xs", md: "xs", lg: "sm" });
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [role, setRole] = useState(null);
  const [email, setEmail] = useState(null);
  const navigate = useNavigate();
  const location = useLocation(); // Получаем текущий маршрут




  const logoutSubmit = async () => {
    const response = await logout();
    if (!response.success) {
      // Обработка ошибок (при необходимости)
    } else {
      localStorage.removeItem("authToken");
      setIsAuthenticated(false);
      setRole(null);
      setEmail(null);
      navigate("/Advertisements/1"); // Перенаправляем на главную
    }
  };
  axios.interceptors.response.use(
    response => response,
    error => {
      if (error.response && error.response.status === 401 &&localStorage.getItem("authToken")!=null) {
        // Удаляем токен из localStorage или другого хранилища
        localStorage.removeItem('authToken');
        window.location.href = '/login';
      }
      return Promise.reject(error);
    }
  );
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
  }, [location]); // Перепроверка при изменении маршрута

  const color = `${COLOR}`;
  const bg = `${BG}`;

   // Состояние для отслеживания открытого аккордеона
   const [expandedItems, setExpandedItems] = useState({});

   const handleAccordionChange = (value) => {
     setExpandedItems((prevState) => ({
       ...prevState,
       [value]: !prevState[value], // Переключение состояния для каждого элемента
     }));
   };
   const navButtonsMobile = (
    <Flex
    direction="row"
    wrap="wrap"
    gap={4}
    align="center"
    flexGrow={1}
   // justify={{ base: "center", md: "space-evenly" }} 
   
  >

    {isAuthenticated && (
      <>
        <Button  color = {color}      _hover={{ 
          color: "black",
            bg: "#FDD835",
          }} size = "xl" variant="link" onClick={() => navigate("/MyAdvertisements/1")} >   {location.pathname === "/MyAdvertisements/1" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )} Мои объявления</Button>

<Button  display={{ base: "none", md: "flex", lg: "flex" }} color = {color} _hover={{ 
          color: "black",
            bg: "#FDD835",
          }} size = "xl" variant="link" onClick={() => navigate("/mySentRentalRequests/1")} >   {location.pathname === "/mySentRentalRequests/1" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )}Отправленные запросы</Button>
          <Button  display={{ base: "none", md: "flex", lg: "flex" }}    color = {color}      _hover={{ 
          color: "black",
            bg: "#FDD835",
          }} size = "xl" variant="link" onClick={() => navigate("/myRentalRequests/1")} >   {location.pathname === "/myRentalRequests/1" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )}Полученные запросы</Button>
        <Button  size = "xl" variant="link" color={color} onClick={() => navigate("/me")}>
        <CgProfile />  {location.pathname === "/me" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )}
      </Button>
        <Button onClick={logoutSubmit} color="white" size="xl" variant="link"  _hover={{ 
            bg: "red",
          }}> 
          Выйти
        </Button>

      </>
    )}
    {!isAuthenticated && (
      <>
        <Button color={color}  display={{ base: "none", md: "flex", lg: "flex"}}  size="xl" variant="link"  ml="auto" right="1" onClick={() => navigate("/login")}          _hover={{ color: "black",
            bg: "#FDD835",
          }} >
          Войти
        </Button>
        <Button color={color}   display={{ base: "none", md: "flex", lg: "flex" }} size="xl" variant="link" onClick={() => navigate("/create-account")}
         _hover={{ 
          color: "black",
            bg: "#FDD835",
          }} >
          Регистрация
        </Button>
      </>
    )}
  </Flex>
  );

   const navButtons = (
    <Flex
    direction="row"
    wrap="wrap"
    gap={4}
    align="center"
    flexGrow={1}
   // justify={{ base: "center", md: "space-evenly" }} 
   
  >
    
    <Button size="2xl" variant="link" left="7" onClick={() => navigate("/Advertisements/1")} //главное меню
      >
      NICEHOME
    </Button>
    {isAuthenticated && (
      <>
      
        <Button  display={{ base: "none", md: "flex", lg: "flex" }}    color = {color}    _hover={{ 
          color: "black",
            bg: "#FDD835",
          }} size = {buttonSize} variant="link" onClick={() => navigate("/create-advertisement")} ml="auto" right="1">   {location.pathname === "/create-advertisement" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )} Создать объявление</Button>
                        <Button  display={{ base: "none", md: "flex", lg: "flex" }}    color = {color}      _hover={{ 
          color: "black",
            bg: "#FDD835",
          }} size = {buttonSize} variant="link" onClick={() => navigate("/MyChats/1")} >   {location.pathname === "/MyChats/1" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )} Мои чаты</Button>
                        <Button  display={{ base: "none", md: "flex", lg: "flex" }}    color = {color}      _hover={{ 
          color: "black",
            bg: "#FDD835",
          }} size = {buttonSize} variant="link" onClick={() => navigate("/MyAdvertisements/1")} >   {location.pathname === "/MyAdvertisements/1" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )} Мои объявления</Button>

<Button  display={{ base: "none", md: "flex", lg: "flex" }}    color = {color}      _hover={{ 
          color: "black",
            bg: "#FDD835",
          }} size = {buttonSize} variant="link" onClick={() => navigate("/mySentRentalRequests/1")} >   {location.pathname === "/mySentRentalRequests/1" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )}Отправленные запросы</Button>
          <Button  display={{ base: "none", md: "flex", lg: "flex" }}    color = {color}      _hover={{ 
          color: "black",
            bg: "#FDD835",
          }} size = {buttonSize} variant="link" onClick={() => navigate("/myRentalRequests/1")} >   {location.pathname === "/myRentalRequests/1" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )}Полученные запросы</Button>
        <Button  size = {buttonSize} variant="link" display={{ base: "none", md: "flex", lg: "flex" }}   _hover={{ 
          color: "black",
            bg: "#FDD835",
          }}   color={color} onClick={() => navigate("/me")}>
        <CgProfile />  {location.pathname === "/me" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )}  Профиль
      </Button>
        <Button onClick={logoutSubmit} color="white" size={buttonSize} variant="link" display={{ base: "none", md: "flex"}}  _hover={{ 
            bg: "red",
          }}> 
          Выйти
        </Button>

      </>
    )}
    {!isAuthenticated && (
      <>
        <Button color={color}  display={{ base: "none", md: "flex", lg: "flex"}}  size={buttonSize} variant="link"  ml="auto" right="1" onClick={() => navigate("/login")}          _hover={{ color: "black",
            bg: "#FDD835",
          }} >
          Войти
        </Button>
        <Button color={color}   display={{ base: "none", md: "flex", lg: "flex" }} size={buttonSize} variant="link" onClick={() => navigate("/create-account")}
         _hover={{ 
          color: "black",
            bg: "#FDD835",
          }} >
          Регистрация
        </Button>
      </>
    )}
  </Flex>
  );
//position="fixed"
  const drawerMenu = (
    <>
<Drawer.Root key="left" placement="left" >  
            <Drawer.Trigger asChild>
              <Button  variant="link"  display={{ base: "flex", md: "none" }} color={color} size={buttonSize}>☰</Button>
            </Drawer.Trigger>
            <Drawer.Positioner>
              <Drawer.Content width={{ base: "50%", md: "16rem" }} height="100vh" boxShadow="lg" zIndex="101" color={color} bg={bg}>
                <Drawer.CloseTrigger />
                <Drawer.Header>
                  <Drawer.Title>Меню</Drawer.Title>
                </Drawer.Header>
                <Drawer.Body>
                  <Accordion.Root>
                    {!isAuthenticated ? (
                      <Accordion.Item>
                        <Accordion.ItemTrigger onClick={() => handleAccordionChange("guest")}>
                          <Box flex="1" textAlign="left">Гость</Box>
                          <Accordion.ItemIndicator />
                        </Accordion.ItemTrigger>
                        {expandedItems["guest"] && (
                          <Accordion.ItemContent>
                            <Accordion.ItemBody>
                              <VStack spacing={2} align="stretch">
                                <Button bg="green" color="white" onClick={() => navigate(`/create-account`)}>  {location.pathname === "/create-account" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )} Создать аккаунт</Button>
                                <Button bg="blue" color="white" onClick={() => navigate(`/login`)}>  {location.pathname === "/login" && (
            <Box
              w="10px"
              h="10px"
              borderRadius="full"
              bg="#FDD835"
              ml={2}
            />
          )} Войти</Button>
                              </VStack>
                            </Accordion.ItemBody>
                          </Accordion.ItemContent>
                        )}
                      </Accordion.Item>
                    ) : (
                      <>
                  <Text fontSize="sm" color={color} mt={2}>
                    {email}
                  </Text>
                        <Accordion.Item>
                          <Accordion.ItemTrigger onClick={() => handleAccordionChange("user")}>
                            <Box flex="1" textAlign="left">Пользователь</Box>
                            <Accordion.ItemIndicator />
                          </Accordion.ItemTrigger>
                          {expandedItems["user"] && (
                            <Accordion.ItemContent>
                              <Accordion.ItemBody>
                                <VStack spacing={2} align="stretch">
                                  <Link to="/me"><Button variant="link">Профиль</Button></Link>
                                  <Link to="/MyChats/1"><Button variant="link">Мои чаты</Button></Link>
                                  <Link to="/change-password"><Button variant="link">Смена пароля</Button></Link>
                                  <Button onClick={logoutSubmit} bg="red" color="white">Выйти</Button>
                                </VStack>
                              </Accordion.ItemBody>
                            </Accordion.ItemContent>
                          )}
                        </Accordion.Item>
  
                        <Accordion.Item>
                          <Accordion.ItemTrigger onClick={() => handleAccordionChange("ads")}>
                            <Box flex="1" textAlign="left">Объявления</Box>
                            <Accordion.ItemIndicator />
                          </Accordion.ItemTrigger>
                          {expandedItems["ads"] && (
                            <Accordion.ItemContent>
                              <Accordion.ItemBody>
                                <VStack spacing={2} align="stretch">
                                  <Link to="/MyAdvertisements/1"><Button variant="link">Мои объявления</Button></Link>
                                  <Link to="/create-advertisement"><Button variant="link">Создать новое объявление</Button></Link>
                                </VStack>
                              </Accordion.ItemBody>
                            </Accordion.ItemContent>
                          )}
                        </Accordion.Item>
  
                        <Accordion.Item>
                          <Accordion.ItemTrigger onClick={() => handleAccordionChange("rentalRequests")}>
                            <Box flex="1" textAlign="left">Запросы на аренду</Box>
                            <Accordion.ItemIndicator />
                          </Accordion.ItemTrigger>
                          {expandedItems["rentalRequests"] && (
                            <Accordion.ItemContent>
                              <Accordion.ItemBody>
                                <VStack spacing={2} align="stretch">
                                  <Link to="/mySentRentalRequests/1"><Button variant="link">Отправленные запросы</Button></Link>
                                  <Link to="/myRentalRequests/1"><Button variant="link">Полученные запросы</Button></Link>
                                </VStack>
                              </Accordion.ItemBody>
                            </Accordion.ItemContent>
                          )}
                        </Accordion.Item>
                        {role === "Admin" && (
                          <Accordion.Item>
                            <Accordion.ItemTrigger onClick={() => handleAccordionChange("admin")}>
                              <Box flex="1" textAlign="left">Администратор</Box>
                              <Accordion.ItemIndicator />
                            </Accordion.ItemTrigger>
                            {expandedItems["admin"] && (
                              <Accordion.ItemContent>
                                <Accordion.ItemBody>
                                  <VStack spacing={2} align="stretch">
                                    <Link to="/users"><Button variant="link">Пользователи</Button></Link>
                                    <Link to="/AllAdvertisements/1"><Button variant="link">Все объявления</Button></Link>
                                    <Link to="/conflicts/1"><Button variant="link">Все отзывы</Button></Link>
                                    <Link to="/conflicts/1"><Button variant="link">Конфликты</Button></Link>      
                                  </VStack>
                                </Accordion.ItemBody>
                              </Accordion.ItemContent>
                            )}
                          </Accordion.Item>
                        )}
                      </>
                    )}
                  </Accordion.Root>
                </Drawer.Body>
                <Drawer.Footer />
              </Drawer.Content>
            </Drawer.Positioner>
          </Drawer.Root>
    </>
  );
 // parseColor("#121212")

 
   return (
    <>
     <Box as="nav"  top="0" w="100%" p={0.2} zIndex="101" color={color} bg={bg} >
        <Flex
    direction="row"
    align="center"
    justify="space-between"
    wrap="wrap"
  >
        <HStack  wrap="wrap" w = "100%">
        {drawerMenu}
        {navButtons}

        </HStack>
      </Flex>
    </Box>
     <Box as="nav"  position="fixed" display={{ base: "flex", md: "none" }}  bottom="0" w="100%" p={0.2} zIndex="101" color={color} bg={bg} >
        <Flex
    direction="row"
    align="center"
    justify="space-between"
    wrap="wrap"
  >
        <HStack  wrap="wrap" w = "100%">
        {navButtonsMobile}
        </HStack>
      </Flex>
     </Box>
    </>
  );
 };
 

const Home = () => (
  <VStack spacing={6} p={6}>
    <h1 className="text-2xl font-bold">Добро пожаловать!</h1>
    <p className="text-gray-600">Выберите действие через меню ☰</p>
  </VStack>
);

const App = () => {
  return (
    <Router>
      <Layout />
      
      <Grid position="fixed top-0 left-0" //templateRows="auto 1fr"  h="120vh"  //w="100vw"     h="150vh" 
    bg = "#FAFAFA" //backgroundImage="url('/public/bgsity.jpg')" // backgroundImage="url('/public/screen-2.jpg')"
  //  backgroundSize="cover"
  //  backgroundPosition="center"
   // backgroundRepeat="no-repeat"
    direction="column" minH="100vh"
    >
      
        <Container 
          //position="absolute"
          px={4} py={6}
          maxW={{ base: "100%", md: "container.md", sm: "container.sm", lg: "container.lg", xl: "container.xl" }}
          overflowY="auto"
          flex="1"
          // bg = "#FAFAFA"
        >
          <Routes>
          <Route path="/Advertisements/:page" element={<GetAdvertisements />} />
          <Route path="/MyAdvertisements/:page" element={<GetMyAdvertisements />} />
          <Route path="/AllAdvertisements/:page" element={<GetAllAdvertisements />} />
          <Route path="/advertisement/:id" element={<GetAdvertisementDetails />} />
          <Route path="/createRentalRequest/:idNeedAdvertisement" element={<CreateRentalRequest />} />
          <Route path="/createReview/:idNeedRentalRequest" element={<CreateReview />} />
          <Route path="/bookingPhotos/:rentalRequestId" element={<GetBookingPhotos />} />
          <Route path="/MyChats/:userId" element={<MyChats />} />
          <Route path="/ChatConversation" element={<ChatConversation />} />
          
          
          
          <Route path="/rentalRequests/:idNeedAdvertisement/:page" element={<GetRentalRequests />} />
          <Route path="/myRentalRequests/:page" element={<GetMyRentalRequests />} />
          <Route path="/mySentRentalRequests/:page" element={<GetMySentRentalRequests />} />
          <Route path="/reviews/:idAdvertisement/:page" element={<GetReviewForAdvertisement />} />
          <Route path="/conflicts/:page" element={<ConflictList />} />
          
            <Route path="/users" element={<GetUsers />} />
            <Route path="/me" element={<Profile />} />
            <Route path="/user/:id" element={<GetUser />} />
            <Route path="/rentalRequest/:id" element={<GetRentalRequest />} />
            <Route path="/create-account" element={<CreateAccount />} />
            <Route path="/login" element={<Login />} />
            <Route path="/change-password" element={<ChangePassword />} /> {/* Добавляем маршрут для смены пароля */}
            <Route path="/create-advertisement" element={<CreateAdvertisement />} /> {/* Добавляем маршрут для смены пароля */}
          </Routes>
        </Container>
      </Grid>
      <Footer/>
    </Router>
  );
};

export default App;