import { Layout } from "@/components/layout/Layout";
import { Hero } from "@/components/sections/Hero";
import { Services } from "@/components/sections/Services";
import { Testimonials } from "@/components/sections/Testimonials";
import { CTA } from "@/components/sections/CTA";

const Index = () => {
  return (
    <Layout>
      <Hero />
      <Services />
      <Testimonials />
      <CTA />
    </Layout>
  );
};

export default Index;
